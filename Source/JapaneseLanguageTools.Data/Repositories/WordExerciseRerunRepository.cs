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

namespace JapaneseLanguageTools.Data.Repositories;

public class WordExerciseRerunRepository : IWordExerciseRerunRepository
{
    private readonly MainDbContext m_context;

    public WordExerciseRerunRepository(MainDbContext context)
    {
        m_context = context;
    }

    /// <inheritdoc />
    public virtual async Task<WordExerciseRerun?> GetWordExerciseRerunAsync(WordExerciseRerunId wordExerciseRerunId, CancellationToken cancellationToken = default)
    {
        WordExerciseRerunFilter wordExerciseRerunFilter = CreateWordExerciseRerunFilter(wordExerciseRerunId);

        WordExerciseRerun? wordExerciseRerun = (await GetWordExerciseRerunsAsync(wordExerciseRerunFilter, cancellationToken)).SingleOrDefault();

        return wordExerciseRerun;
    }

    /// <inheritdoc />
    public virtual async Task<WordExerciseRerun[]> GetWordExerciseRerunsAsync(IEnumerable<WordExerciseRerunId> wordExerciseRerunIds, CancellationToken cancellationToken = default)
    {
        WordExerciseRerunFilter wordExerciseRerunFilter = CreateWordExerciseRerunFilter(wordExerciseRerunIds);

        WordExerciseRerun[] wordExerciseReruns = await GetWordExerciseRerunsAsync(wordExerciseRerunFilter, cancellationToken: cancellationToken);

        return wordExerciseReruns;
    }

    /// <inheritdoc />
    public virtual async Task<WordExerciseRerun[]> GetWordExerciseRerunsAsync(WordExerciseRerunFilter wordExerciseRerunFilter, CancellationToken cancellationToken = default)
    {
        return await GetWordExerciseRerunsAsync(wordExerciseRerunFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<WordExerciseRerun[]> GetWordExerciseRerunsAsync(WordExerciseRerunFilter? wordExerciseRerunFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<WordExerciseRerun> wordExerciseRerunsQueryable = m_context.WordExerciseReruns;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        wordExerciseRerunsQueryable = wordExerciseRerunsQueryable.Include(wordExerciseRerun => wordExerciseRerun.WordExercise.Word.WordGroup);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        wordExerciseRerunsQueryable = wordExerciseRerunsQueryable.Include(wordExerciseRerun => wordExerciseRerun.WordExercise.Word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (wordExerciseRerunFilter is not null)
            wordExerciseRerunsQueryable = wordExerciseRerunFilter.Filter(wordExerciseRerunsQueryable);

        if (asNoTracking)
            wordExerciseRerunsQueryable = wordExerciseRerunsQueryable.AsNoTracking();

        wordExerciseRerunsQueryable = wordExerciseRerunsQueryable.AsSingleQuery();

        WordExerciseRerun[] wordExerciseReruns = await wordExerciseRerunsQueryable.ToArrayAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveNavigationPropertyCycles(wordExerciseReruns.Select(wordExerciseRerun => wordExerciseRerun.WordExercise));
            RemoveNavigationPropertyCycles(wordExerciseReruns.Select(wordExerciseRerun => wordExerciseRerun.WordExercise?.Word));
        }

        return wordExerciseReruns;
    }

    /// <inheritdoc />
    public virtual async Task<WordExerciseRerun[]> GetAllWordExerciseRerunsAsync(CancellationToken cancellationToken = default)
    {
        return await GetWordExerciseRerunsAsync(wordExerciseRerunFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<WordExerciseRerun> AddWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, CancellationToken cancellationToken = default)
    {
        return await AddWordExerciseRerunAsync(wordExerciseRerun, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<WordExerciseRerun> AddWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        wordExerciseRerun.Id = default(int);

        m_context.WordExerciseReruns.Entry(wordExerciseRerun).State = EntityState.Added;

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return wordExerciseRerun;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, CancellationToken cancellationToken = default)
    {
        return await UpdateWordExerciseRerunAsync(wordExerciseRerun, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> UpdateWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        WordExerciseRerunFilter wordExerciseRerunFilter = CreateWordExerciseRerunFilter(new WordExerciseRerunId(wordExerciseRerun.Id));

        WordExerciseRerun? existingWordExerciseRerun = (await GetWordExerciseRerunsAsync(wordExerciseRerunFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordExerciseRerun is null)
            return false;

        existingWordExerciseRerun.WordExerciseId = wordExerciseRerun.WordExerciseId;

        existingWordExerciseRerun.RequiredChallengeCount = wordExerciseRerun.RequiredChallengeCount;
        existingWordExerciseRerun.ContinuousChallengeCount = wordExerciseRerun.ContinuousChallengeCount;
        existingWordExerciseRerun.TotalChallengeCount = wordExerciseRerun.TotalChallengeCount;
        existingWordExerciseRerun.InitiallyScheduledOn = wordExerciseRerun.InitiallyScheduledOn;
        existingWordExerciseRerun.RepeatedlyScheduledOn = wordExerciseRerun.RepeatedlyScheduledOn;

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordExerciseRerunAsync(WordExerciseRerunId wordExerciseRerunId, CancellationToken cancellationToken = default)
    {
        WordExerciseRerunFilter wordExerciseRerunFilter = CreateWordExerciseRerunFilter(wordExerciseRerunId);

        WordExerciseRerun? existingWordExerciseRerun = (await GetWordExerciseRerunsAsync(wordExerciseRerunFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordExerciseRerun is null)
            return false;

        bool removed = await RemoveWordExerciseRerunAsync(existingWordExerciseRerun, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, CancellationToken cancellationToken = default)
    {
        return await RemoveWordExerciseRerunAsync(wordExerciseRerun, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> RemoveWordExerciseRerunAsync(WordExerciseRerun wordExerciseRerun, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        m_context.WordExerciseReruns.Remove(wordExerciseRerun);

        int rowsRemoved = 0;
        if (saveChangesImmediately)
            rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return !saveChangesImmediately || rowsRemoved > 0;
    }

    protected static WordExerciseRerunFilter CreateWordExerciseRerunFilter(WordExerciseRerunId wordExerciseRerunId)
    {
        int wordExerciseRerunIdRaw = wordExerciseRerunId.Value;

        IQueryable<WordExerciseRerun> CustomFilter(IQueryable<WordExerciseRerun> wordExerciseReruns) =>
            wordExerciseReruns.Where(wordExerciseRerun => wordExerciseRerun.Id == wordExerciseRerunIdRaw);

        WordExerciseRerunFilter wordExerciseRerunFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordExerciseRerunFilter;
    }

    protected static WordExerciseRerunFilter CreateWordExerciseRerunFilter(IEnumerable<WordExerciseRerunId> wordExerciseRerunIds)
    {
        IEnumerable<int> wordExerciseRerunIdsRaw = wordExerciseRerunIds.Select(wordExerciseRerunId => wordExerciseRerunId.Value);

        IQueryable<WordExerciseRerun> CustomFilter(IQueryable<WordExerciseRerun> wordExerciseReruns) =>
            wordExerciseReruns.Where(wordExerciseRerun => wordExerciseRerunIdsRaw.Contains(wordExerciseRerun.Id));

        WordExerciseRerunFilter wordExerciseRerunFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordExerciseRerunFilter;
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<WordExercise?> wordExercises)
    {
        foreach (WordExercise? wordExercise in wordExercises)
        {
            if (wordExercise?.WordExerciseReruns.Count > 0)
                wordExercise.WordExerciseReruns = [];

            ;
        }
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<Word?> words)
    {
        foreach (Word? word in words)
        {
            if (word?.WordGroup?.Words.Count > 0)
                word.WordGroup.Words = [];

            ;
        }
    }
}
