using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

using Microsoft.EntityFrameworkCore;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Data.Repositories;

public class WordRepository : IWordRepository
{
    private readonly MainDbContext m_context;
    private readonly TimeProvider m_timeProvider;

    public WordRepository(MainDbContext context, TimeProvider timeProvider)
    {
        m_context = context;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public virtual async Task<Word?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        WordFilter wordFilter = CreateWordFilter(wordId);

        Word? word = (await GetWordsAsync(wordFilter, cancellationToken)).SingleOrDefault();

        return word;
    }

    /// <inheritdoc />
    public virtual async Task<Word[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default)
    {
        WordFilter wordFilter = CreateWordFilter(wordIds);

        Word[] words = await GetWordsAsync(wordFilter, asNoTracking: true, cancellationToken: cancellationToken);

        return words;
    }

    /// <inheritdoc />
    public virtual async Task<Word[]> GetWordsAsync(WordFilter wordFilter, CancellationToken cancellationToken = default)
    {
        return await GetWordsAsync(wordFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<Word[]> GetWordsAsync(WordFilter? wordFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Word> wordsQueryable = m_context.Words;

        wordsQueryable = wordsQueryable.Include(word => word.WordGroup);

        wordsQueryable = wordsQueryable.Include(word => word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);

        if (wordFilter is not null)
            wordsQueryable = wordFilter.Filter(wordsQueryable);

        if (asNoTracking)
            wordsQueryable = wordsQueryable.AsNoTracking();

        wordsQueryable = wordsQueryable.AsSingleQuery();

        Word[] words = await wordsQueryable.ToArrayAsync(cancellationToken);

        return words;
    }

    /// <inheritdoc />
    public virtual async Task<Word[]> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        return await GetWordsAsync(wordFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<Word> AddWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await AddWordAsync(word, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<Word> AddWordAsync(Word word, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        word.Id = default(int);
        foreach (WordTag wordTag in word.WordTags)
            wordTag.WordId = default(int);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        word.CreatedOn = word.CreatedOn != default(DateTimeOffset)
            ? word.CreatedOn
            : utcNow;
        word.UpdatedOn = word.UpdatedOn != default(DateTimeOffset)
            ? word.UpdatedOn
            : utcNow;

        m_context.Words.Entry(word).State = EntityState.Added;

        foreach (WordTag wordTag in word.WordTags)
            m_context.WordTags.Entry(wordTag).State = EntityState.Added;

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return word;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await UpdateWordAsync(word, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> UpdateWordAsync(Word word, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        WordFilter wordFilter = CreateWordFilter(new WordId(word.Id));

        Word? existingWord = (await GetWordsAsync(wordFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWord is null)
            return false;

        existingWord.WordGroupId = word.WordGroupId;

        existingWord.Characters = word.Characters;
        existingWord.CharacterTypes = word.CharacterTypes;
        existingWord.Pronunciation = word.Pronunciation;
        existingWord.Furigana = word.Furigana;
        existingWord.Okurigana = word.Okurigana;
        existingWord.Meaning = word.Meaning;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingWord.UpdatedOn = word.UpdatedOn != default(DateTimeOffset)
            ? word.UpdatedOn
            : utcNow;

        MergeWordTags(existingWord.WordTags, word.WordTags);

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        WordFilter wordFilter = CreateWordFilter(wordId);

        Word? existingWord = (await GetWordsAsync(wordFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWord is null)
            return false;

        bool removed = await RemoveWordAsync(existingWord, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await RemoveWordAsync(word, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> RemoveWordAsync(Word word, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        m_context.Words.Remove(word);

        int rowsRemoved = 0;
        if (saveChangesImmediately)
            rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return !saveChangesImmediately || rowsRemoved > 0;
    }

    protected void MergeWordTags(IEnumerable<WordTag> targetWordTags, IEnumerable<WordTag> sourceWordTags)
    {
        HashSet<(int WordId, int TagId)> targetWordTagIds = targetWordTags
            .Select(wordTag => (wordTag.WordId, wordTag.TagId))
            .ToHashSet();
        HashSet<(int WordId, int TagId)> sourceWordTagIds = sourceWordTags
            .Select(wordTag => (wordTag.WordId, wordTag.TagId))
            .ToHashSet();

        foreach (WordTag sourceWordTag in sourceWordTags)
        {
            // The 'when not matched by target' clause:
            if (!targetWordTagIds.Contains((sourceWordTag.WordId, sourceWordTag.TagId)))
                m_context.WordTags.Entry(sourceWordTag).State = EntityState.Added;

            // The WordTag entity does not have any fields except the primary key,
            // therefore no 'when matched' clause.
        }

        foreach (WordTag targetWordTag in targetWordTags)
        {
            // The 'when not matched by source' clause:
            if (!sourceWordTagIds.Contains((targetWordTag.WordId, targetWordTag.TagId)))
                m_context.WordTags.Entry(targetWordTag).State = EntityState.Deleted;

            ;
        }
    }

    protected static WordFilter CreateWordFilter(WordId wordId)
    {
        int wordIdRaw = wordId.Value;

        IQueryable<Word> CustomFilter(IQueryable<Word> words) =>
            words.Where(word => word.Id == wordIdRaw);

        WordFilter wordFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordFilter;
    }

    protected static WordFilter CreateWordFilter(IEnumerable<WordId> wordIds)
    {
        IEnumerable<int> wordIdsRaw = wordIds.Select(wordId => wordId.Value);

        IQueryable<Word> CustomFilter(IQueryable<Word> words) =>
            words.Where(word => wordIdsRaw.Contains(word.Id));

        WordFilter wordFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordFilter;
    }
}
