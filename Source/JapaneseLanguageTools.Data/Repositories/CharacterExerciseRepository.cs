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

public class CharacterExerciseRepository : ICharacterExerciseRepository
{
    private readonly MainDbContext m_context;
    private readonly CharacterExerciseRerunRepository m_characterExerciseRerunRepository;

    public CharacterExerciseRepository(MainDbContext context, CharacterExerciseRerunRepository characterExerciseRerunRepository)
    {
        m_context = context;
        m_characterExerciseRerunRepository = characterExerciseRerunRepository;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExercise?> GetCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default)
    {
        CharacterExerciseFilter characterExerciseFilter = CreateCharacterExerciseFilter(characterExerciseId);

        CharacterExercise? characterExercise = (await GetCharacterExercisesAsync(characterExerciseFilter, cancellationToken)).SingleOrDefault();

        return characterExercise;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExercise[]> GetCharacterExercisesAsync(IEnumerable<CharacterExerciseId> characterExerciseIds, CancellationToken cancellationToken = default)
    {
        CharacterExerciseFilter characterExerciseFilter = CreateCharacterExerciseFilter(characterExerciseIds);

        CharacterExercise[] characterExercises = await GetCharacterExercisesAsync(characterExerciseFilter, asNoTracking: true, cancellationToken: cancellationToken);

        return characterExercises;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExercise[]> GetCharacterExercisesAsync(CharacterExerciseFilter characterExerciseFilter, CancellationToken cancellationToken = default)
    {
        return await GetCharacterExercisesAsync(characterExerciseFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<CharacterExercise[]> GetCharacterExercisesAsync(CharacterExerciseFilter? characterExerciseFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<CharacterExercise> characterExercisesQueryable = m_context.CharacterExercises;

        characterExercisesQueryable = characterExercisesQueryable.Include(characterExercise => characterExercise.CharacterExerciseReruns);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        characterExercisesQueryable = characterExercisesQueryable.Include(characterExercise => characterExercise.Character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (characterExerciseFilter is not null)
            characterExercisesQueryable = characterExerciseFilter.Filter(characterExercisesQueryable);

        if (asNoTracking)
            characterExercisesQueryable = characterExercisesQueryable.AsNoTracking();

        characterExercisesQueryable = characterExercisesQueryable.AsSingleQuery();

        CharacterExercise[] characterExercises = await characterExercisesQueryable.ToArrayAsync(cancellationToken);

        return characterExercises;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExercise[]> GetAllCharacterExercisesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCharacterExercisesAsync(characterExerciseFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExercise> AddCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default)
    {
        characterExercise.Id = default(int);

        m_context.CharacterExercises.Entry(characterExercise).State = EntityState.Added;

        foreach (CharacterExerciseRerun characterExerciseRerun in characterExercise.CharacterExerciseReruns)
            await m_characterExerciseRerunRepository.AddCharacterExerciseRerunAsync(characterExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return characterExercise;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default)
    {
        CharacterExerciseFilter characterExerciseFilter = CreateCharacterExerciseFilter(new CharacterExerciseId(characterExercise.Id));

        CharacterExercise? existingCharacterExercise = (await GetCharacterExercisesAsync(characterExerciseFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterExercise is null)
            return false;

        existingCharacterExercise.InstanceData = characterExercise.InstanceData;

        await MergeCharacterExerciseRerunsAsync(existingCharacterExercise.CharacterExerciseReruns, characterExercise.CharacterExerciseReruns, cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterExerciseAsync(CharacterExerciseId characterExerciseId, CancellationToken cancellationToken = default)
    {
        CharacterExerciseFilter characterExerciseFilter = CreateCharacterExerciseFilter(characterExerciseId);

        CharacterExercise? existingCharacterExercise = (await GetCharacterExercisesAsync(characterExerciseFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterExercise is null)
            return false;

        bool removed = await RemoveCharacterExerciseAsync(existingCharacterExercise, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterExerciseAsync(CharacterExercise characterExercise, CancellationToken cancellationToken = default)
    {
        foreach (CharacterExerciseRerun characterExerciseRerun in characterExercise.CharacterExerciseReruns)
            await m_characterExerciseRerunRepository.RemoveCharacterExerciseRerunAsync(characterExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);

        m_context.CharacterExercises.Remove(characterExercise);

        int rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return rowsRemoved > 0;
    }

    protected async Task MergeCharacterExerciseRerunsAsync(IEnumerable<CharacterExerciseRerun> targetCharacterExerciseReruns, IEnumerable<CharacterExerciseRerun> sourceCharacterExerciseReruns, CancellationToken cancellationToken = default)
    {
        HashSet<int> targetCharacterExerciseRerunIds = targetCharacterExerciseReruns
            .Select(characterExerciseRerun => characterExerciseRerun.Id)
            .ToHashSet();
        HashSet<int> sourceCharacterExerciseRerunIds = sourceCharacterExerciseReruns
            .Select(characterExerciseRerun => characterExerciseRerun.Id)
            .ToHashSet();

        foreach (CharacterExerciseRerun sourceCharacterExerciseRerun in sourceCharacterExerciseReruns)
        {
            // The 'when not matched by target' clause:
            if (!targetCharacterExerciseRerunIds.Contains(sourceCharacterExerciseRerun.Id))
            {
                await m_characterExerciseRerunRepository.AddCharacterExerciseRerunAsync(sourceCharacterExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
            // The 'when matched' clause:
            else // if (targetCharacterExerciseRerunIds.Contains(sourceCharacterExerciseRerun.Id))
            {
                await m_characterExerciseRerunRepository.UpdateCharacterExerciseRerunAsync(sourceCharacterExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
        }

        foreach (CharacterExerciseRerun targetCharacterExerciseRerun in targetCharacterExerciseReruns)
        {
            // The 'when not matched by source' clause:
            if (!sourceCharacterExerciseRerunIds.Contains(targetCharacterExerciseRerun.Id))
                await m_characterExerciseRerunRepository.RemoveCharacterExerciseRerunAsync(targetCharacterExerciseRerun, saveChangesImmediately: false, cancellationToken: cancellationToken);

            ;
        }
    }

    protected static CharacterExerciseFilter CreateCharacterExerciseFilter(CharacterExerciseId characterExerciseId)
    {
        int characterExerciseIdRaw = characterExerciseId.Value;

        IQueryable<CharacterExercise> CustomFilter(IQueryable<CharacterExercise> characterExercises) =>
            characterExercises.Where(characterExercise => characterExercise.Id == characterExerciseIdRaw);

        CharacterExerciseFilter characterExerciseFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterExerciseFilter;
    }

    protected static CharacterExerciseFilter CreateCharacterExerciseFilter(IEnumerable<CharacterExerciseId> characterExerciseIds)
    {
        IEnumerable<int> characterExerciseIdsRaw = characterExerciseIds.Select(characterExerciseId => characterExerciseId.Value);

        IQueryable<CharacterExercise> CustomFilter(IQueryable<CharacterExercise> characterExercises) =>
            characterExercises.Where(characterExercise => characterExerciseIdsRaw.Contains(characterExercise.Id));

        CharacterExerciseFilter characterExerciseFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterExerciseFilter;
    }
}
