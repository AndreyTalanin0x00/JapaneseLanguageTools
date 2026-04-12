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

public class WordGroupRepository : IWordGroupRepository
{
    private readonly MainDbContext m_context;
    private readonly WordRepository m_wordRepository;
    private readonly TimeProvider m_timeProvider;

    public WordGroupRepository(MainDbContext context, WordRepository wordRepository, TimeProvider timeProvider)
    {
        m_context = context;
        m_wordRepository = wordRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(wordGroupId);

        WordGroup? wordGroup = (await GetWordGroupsAsync(wordGroupFilter, cancellationToken)).SingleOrDefault();

        return wordGroup;
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(wordGroupCaption);

        WordGroup? wordGroup = (await GetWordGroupsAsync(wordGroupFilter, cancellationToken)).SingleOrDefault();

        return wordGroup;
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(wordGroupIds);

        WordGroup[] wordGroups = await GetWordGroupsAsync(wordGroupFilter, cancellationToken: cancellationToken);

        return wordGroups;
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(wordGroupCaptions);

        WordGroup[] wordGroups = await GetWordGroupsAsync(wordGroupCaptions, cancellationToken: cancellationToken);

        return wordGroups;
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup[]> GetWordGroupsAsync(WordGroupFilter wordGroupFilter, CancellationToken cancellationToken = default)
    {
        return await GetWordGroupsAsync(wordGroupFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<WordGroup[]> GetWordGroupsAsync(WordGroupFilter? wordGroupFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<WordGroup> wordGroupsQueryable = m_context.WordGroups;

        wordGroupsQueryable = wordGroupsQueryable.Include(wordGroup => wordGroup.Words)
            .ThenInclude(word => word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);

        if (wordGroupFilter is not null)
            wordGroupsQueryable = wordGroupFilter.Filter(wordGroupsQueryable);

        if (asNoTracking)
            wordGroupsQueryable = wordGroupsQueryable.AsNoTracking();

        wordGroupsQueryable = wordGroupsQueryable.AsSingleQuery();

        WordGroup[] wordGroups = await wordGroupsQueryable.ToArrayAsync(cancellationToken);

        return wordGroups;
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await GetWordGroupsAsync(wordGroupFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<WordGroup> AddWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        wordGroup.Id = default(int);
        foreach (Word word in wordGroup.Words)
            word.WordGroupId = default(int);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        wordGroup.CreatedOn = wordGroup.CreatedOn != default(DateTimeOffset)
            ? wordGroup.CreatedOn
            : utcNow;
        wordGroup.UpdatedOn = wordGroup.UpdatedOn != default(DateTimeOffset)
            ? wordGroup.UpdatedOn
            : utcNow;

        m_context.WordGroups.Entry(wordGroup).State = EntityState.Added;

        foreach (Word word in wordGroup.Words)
            await m_wordRepository.AddWordAsync(word, saveChangesImmediately: false, cancellationToken: cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return wordGroup;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(new WordGroupId(wordGroup.Id));

        WordGroup? existingWordGroup = (await GetWordGroupsAsync(wordGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordGroup is null)
            return false;

        existingWordGroup.Caption = wordGroup.Caption;
        existingWordGroup.Comment = wordGroup.Comment;
        existingWordGroup.Enabled = wordGroup.Enabled;
        existingWordGroup.AlwaysUse = wordGroup.AlwaysUse;
        existingWordGroup.Hidden = wordGroup.Hidden;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingWordGroup.UpdatedOn = wordGroup.UpdatedOn != default(DateTimeOffset)
            ? wordGroup.UpdatedOn
            : utcNow;

        await MergeWordsAsync(existingWordGroup.Words, wordGroup.Words, cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> ChangeWordGroupStateAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(new WordGroupId(wordGroup.Id));

        WordGroup? existingWordGroup = (await GetWordGroupsAsync(wordGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordGroup is null)
            return false;

        existingWordGroup.Enabled = wordGroup.Enabled;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingWordGroup.UpdatedOn = wordGroup.UpdatedOn != default(DateTimeOffset)
            ? wordGroup.UpdatedOn
            : utcNow;

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(wordGroupId);

        WordGroup? existingWordGroup = (await GetWordGroupsAsync(wordGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordGroup is null)
            return false;

        bool removed = await RemoveWordGroupAsync(existingWordGroup, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        WordGroupFilter wordGroupFilter = CreateWordGroupFilter(wordGroupCaption);

        WordGroup? existingWordGroup = (await GetWordGroupsAsync(wordGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordGroup is null)
            return false;

        bool removed = await RemoveWordGroupAsync(existingWordGroup, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        foreach (Word word in wordGroup.Words)
            await m_wordRepository.RemoveWordAsync(word, saveChangesImmediately: false, cancellationToken: cancellationToken);

        m_context.WordGroups.Remove(wordGroup);

        int rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return rowsRemoved > 0;
    }

    protected async Task MergeWordsAsync(IEnumerable<Word> targetWords, IEnumerable<Word> sourceWords, CancellationToken cancellationToken = default)
    {
        HashSet<int> targetWordIds = targetWords
            .Select(word => word.Id)
            .ToHashSet();
        HashSet<int> sourceWordIds = sourceWords
            .Select(word => word.Id)
            .ToHashSet();

        foreach (Word sourceWord in sourceWords)
        {
            // The 'when not matched by target' clause:
            if (!targetWordIds.Contains(sourceWord.Id))
            {
                await m_wordRepository.AddWordAsync(sourceWord, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
            // The 'when matched' clause:
            else // if (targetWordIds.Contains(sourceWord.Id))
            {
                await m_wordRepository.UpdateWordAsync(sourceWord, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
        }

        foreach (Word targetWord in targetWords)
        {
            // The 'when not matched by source' clause:
            if (!sourceWordIds.Contains(targetWord.Id))
                await m_wordRepository.RemoveWordAsync(targetWord, saveChangesImmediately: false, cancellationToken: cancellationToken);

            ;
        }
    }

    protected static WordGroupFilter CreateWordGroupFilter(WordGroupId wordGroupId)
    {
        int wordGroupIdRaw = wordGroupId.Value;

        IQueryable<WordGroup> CustomFilter(IQueryable<WordGroup> wordGroups) =>
            wordGroups.Where(wordGroup => wordGroup.Id == wordGroupIdRaw);

        WordGroupFilter wordGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordGroupFilter;
    }

    protected static WordGroupFilter CreateWordGroupFilter(WordGroupCaption wordGroupCaption)
    {
        string wordGroupCaptionRaw = wordGroupCaption.Value;

        IQueryable<WordGroup> CustomFilter(IQueryable<WordGroup> wordGroups) =>
            wordGroups.Where(wordGroup => wordGroup.Caption == wordGroupCaptionRaw);

        WordGroupFilter wordGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordGroupFilter;
    }

    protected static WordGroupFilter CreateWordGroupFilter(IEnumerable<WordGroupId> wordGroupIds)
    {
        IEnumerable<int> wordGroupIdsRaw = wordGroupIds.Select(wordGroupId => wordGroupId.Value);

        IQueryable<WordGroup> CustomFilter(IQueryable<WordGroup> wordGroups) =>
            wordGroups.Where(wordGroup => wordGroupIdsRaw.Contains(wordGroup.Id));

        WordGroupFilter wordGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordGroupFilter;
    }

    protected static WordGroupFilter CreateWordGroupFilter(IEnumerable<WordGroupCaption> wordGroupCaptions)
    {
        IEnumerable<string> wordGroupCaptionsRaw = wordGroupCaptions.Select(wordGroupCaption => wordGroupCaption.Value);

        IQueryable<WordGroup> CustomFilter(IQueryable<WordGroup> wordGroups) =>
            wordGroups.Where(wordGroup => wordGroupCaptionsRaw.Contains(wordGroup.Caption));

        WordGroupFilter wordGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordGroupFilter;
    }
}
