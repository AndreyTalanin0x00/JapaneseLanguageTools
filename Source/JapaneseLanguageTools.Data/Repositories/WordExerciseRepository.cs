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

public class WordExerciseRepository : IWordExerciseRepository
{
    private readonly MainDbContext m_context;
    private readonly WordExerciseRerunRepository m_wordExerciseRerunRepository;

    public WordExerciseRepository(MainDbContext context, WordExerciseRerunRepository wordExerciseRerunRepository)
    {
        m_context = context;
        m_wordExerciseRerunRepository = wordExerciseRerunRepository;
    }

    /// <inheritdoc />
    public virtual async Task<WordExercise?> GetWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default)
    {
        WordExerciseFilter wordExerciseFilter = CreateWordExerciseFilter(wordExerciseId);

        WordExercise? wordExercise = (await GetWordExercisesAsync(wordExerciseFilter, cancellationToken)).SingleOrDefault();

        return wordExercise;
    }

    /// <inheritdoc />
    public virtual async Task<WordExercise[]> GetWordExercisesAsync(IEnumerable<WordExerciseId> wordExerciseIds, CancellationToken cancellationToken = default)
    {
        WordExerciseFilter wordExerciseFilter = CreateWordExerciseFilter(wordExerciseIds);

        WordExercise[] wordExercises = await GetWordExercisesAsync(wordExerciseFilter, asNoTracking: true, cancellationToken: cancellationToken);

        return wordExercises;
    }

    /// <inheritdoc />
    public virtual async Task<WordExercise[]> GetWordExercisesAsync(WordExerciseFilter wordExerciseFilter, CancellationToken cancellationToken = default)
    {
        return await GetWordExercisesAsync(wordExerciseFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<WordExercise[]> GetWordExercisesAsync(WordExerciseFilter? wordExerciseFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<WordExercise> wordExercisesQueryable = m_context.WordExercises;

        wordExercisesQueryable = wordExercisesQueryable.Include(wordExercise => wordExercise.WordExerciseReruns);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        wordExercisesQueryable = wordExercisesQueryable.Include(wordExercise => wordExercise.Word.WordTags)
            .ThenInclude(wordTag => wordTag.Tag);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (wordExerciseFilter is not null)
            wordExercisesQueryable = wordExerciseFilter.Filter(wordExercisesQueryable);

        if (asNoTracking)
            wordExercisesQueryable = wordExercisesQueryable.AsNoTracking();

        wordExercisesQueryable = wordExercisesQueryable.AsSingleQuery();

        WordExercise[] wordExercises = await wordExercisesQueryable.ToArrayAsync(cancellationToken);

        return wordExercises;
    }

    /// <inheritdoc />
    public virtual async Task<WordExercise[]> GetAllWordExercisesAsync(CancellationToken cancellationToken = default)
    {
        return await GetWordExercisesAsync(wordExerciseFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<WordExercise> AddWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default)
    {
        wordExercise.Id = default(int);

        m_context.WordExercises.Entry(wordExercise).State = EntityState.Added;

        foreach (WordExerciseRerun wordExerciseRerun in wordExercise.WordExerciseReruns)
            await m_wordExerciseRerunRepository.AddWordExerciseRerunAsync(wordExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return wordExercise;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default)
    {
        WordExerciseFilter wordExerciseFilter = CreateWordExerciseFilter(new WordExerciseId(wordExercise.Id));

        WordExercise? existingWordExercise = (await GetWordExercisesAsync(wordExerciseFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordExercise is null)
            return false;

        existingWordExercise.InstanceData = wordExercise.InstanceData;

        await MergeWordExerciseRerunsAsync(existingWordExercise.WordExerciseReruns, wordExercise.WordExerciseReruns, cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordExerciseAsync(WordExerciseId wordExerciseId, CancellationToken cancellationToken = default)
    {
        WordExerciseFilter wordExerciseFilter = CreateWordExerciseFilter(wordExerciseId);

        WordExercise? existingWordExercise = (await GetWordExercisesAsync(wordExerciseFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingWordExercise is null)
            return false;

        bool removed = await RemoveWordExerciseAsync(existingWordExercise, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveWordExerciseAsync(WordExercise wordExercise, CancellationToken cancellationToken = default)
    {
        foreach (WordExerciseRerun wordExerciseRerun in wordExercise.WordExerciseReruns)
            await m_wordExerciseRerunRepository.RemoveWordExerciseRerunAsync(wordExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);

        m_context.WordExercises.Remove(wordExercise);

        int rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return rowsRemoved > 0;
    }

    protected async Task MergeWordExerciseRerunsAsync(IEnumerable<WordExerciseRerun> targetWordExerciseReruns, IEnumerable<WordExerciseRerun> sourceWordExerciseReruns, CancellationToken cancellationToken = default)
    {
        HashSet<int> targetWordExerciseRerunIds = targetWordExerciseReruns
            .Select(wordExerciseRerun => wordExerciseRerun.Id)
            .ToHashSet();
        HashSet<int> sourceWordExerciseRerunIds = sourceWordExerciseReruns
            .Select(wordExerciseRerun => wordExerciseRerun.Id)
            .ToHashSet();

        foreach (WordExerciseRerun sourceWordExerciseRerun in sourceWordExerciseReruns)
        {
            // The 'when not matched by target' clause:
            if (!targetWordExerciseRerunIds.Contains(sourceWordExerciseRerun.Id))
            {
                await m_wordExerciseRerunRepository.AddWordExerciseRerunAsync(sourceWordExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
            // The 'when matched' clause:
            else // if (targetWordExerciseRerunIds.Contains(sourceWordExerciseRerun.Id))
            {
                await m_wordExerciseRerunRepository.UpdateWordExerciseRerunAsync(sourceWordExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
        }

        foreach (WordExerciseRerun targetWordExerciseRerun in targetWordExerciseReruns)
        {
            // The 'when not matched by source' clause:
            if (!sourceWordExerciseRerunIds.Contains(targetWordExerciseRerun.Id))
                await m_wordExerciseRerunRepository.RemoveWordExerciseRerunAsync(targetWordExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);

            ;
        }
    }

    protected static WordExerciseFilter CreateWordExerciseFilter(WordExerciseId wordExerciseId)
    {
        int wordExerciseIdRaw = wordExerciseId.Value;

        IQueryable<WordExercise> CustomFilter(IQueryable<WordExercise> wordExercises) =>
            wordExercises.Where(wordExercise => wordExercise.Id == wordExerciseIdRaw);

        WordExerciseFilter wordExerciseFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordExerciseFilter;
    }

    protected static WordExerciseFilter CreateWordExerciseFilter(IEnumerable<WordExerciseId> wordExerciseIds)
    {
        IEnumerable<int> wordExerciseIdsRaw = wordExerciseIds.Select(wordExerciseId => wordExerciseId.Value);

        IQueryable<WordExercise> CustomFilter(IQueryable<WordExercise> wordExercises) =>
            wordExercises.Where(wordExercise => wordExerciseIdsRaw.Contains(wordExercise.Id));

        WordExerciseFilter wordExerciseFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return wordExerciseFilter;
    }
}
