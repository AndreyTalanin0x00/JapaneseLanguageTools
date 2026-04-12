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

public class CharacterExerciseRerunRepository : ICharacterExerciseRerunRepository
{
    private readonly MainDbContext m_context;

    public CharacterExerciseRerunRepository(MainDbContext context)
    {
        m_context = context;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExerciseRerun?> GetCharacterExerciseRerunAsync(CharacterExerciseRerunId characterExerciseRerunId, CancellationToken cancellationToken = default)
    {
        CharacterExerciseRerunFilter characterExerciseRerunFilter = CreateCharacterExerciseRerunFilter(characterExerciseRerunId);

        CharacterExerciseRerun? characterExerciseRerun = (await GetCharacterExerciseRerunsAsync(characterExerciseRerunFilter, cancellationToken)).SingleOrDefault();

        return characterExerciseRerun;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExerciseRerun[]> GetCharacterExerciseRerunsAsync(IEnumerable<CharacterExerciseRerunId> characterExerciseRerunIds, CancellationToken cancellationToken = default)
    {
        CharacterExerciseRerunFilter characterExerciseRerunFilter = CreateCharacterExerciseRerunFilter(characterExerciseRerunIds);

        CharacterExerciseRerun[] characterExerciseReruns = await GetCharacterExerciseRerunsAsync(characterExerciseRerunFilter, cancellationToken);

        return characterExerciseReruns;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExerciseRerun[]> GetCharacterExerciseRerunsAsync(CharacterExerciseRerunFilter characterExerciseRerunFilter, CancellationToken cancellationToken = default)
    {
        return await GetCharacterExerciseRerunsAsync(characterExerciseRerunFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<CharacterExerciseRerun[]> GetCharacterExerciseRerunsAsync(CharacterExerciseRerunFilter? characterExerciseRerunFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<CharacterExerciseRerun> characterExerciseRerunsQueryable = m_context.CharacterExerciseReruns;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        characterExerciseRerunsQueryable = characterExerciseRerunsQueryable.Include(characterExerciseRerun => characterExerciseRerun.CharacterExercise.Character.CharacterGroup);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        characterExerciseRerunsQueryable = characterExerciseRerunsQueryable.Include(characterExerciseRerun => characterExerciseRerun.CharacterExercise.Character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (characterExerciseRerunFilter is not null)
            characterExerciseRerunsQueryable = characterExerciseRerunFilter.Filter(characterExerciseRerunsQueryable);

        if (asNoTracking)
            characterExerciseRerunsQueryable = characterExerciseRerunsQueryable.AsNoTracking();

        characterExerciseRerunsQueryable = characterExerciseRerunsQueryable.AsSingleQuery();

        CharacterExerciseRerun[] characterExerciseReruns = await characterExerciseRerunsQueryable.ToArrayAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveNavigationPropertyCycles(characterExerciseReruns.Select(characterExerciseRerun => characterExerciseRerun.CharacterExercise));
            RemoveNavigationPropertyCycles(characterExerciseReruns.Select(characterExerciseRerun => characterExerciseRerun.CharacterExercise?.Character));
        }

        return characterExerciseReruns;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExerciseRerun[]> GetAllCharacterExerciseRerunsAsync(CancellationToken cancellationToken = default)
    {
        return await GetCharacterExerciseRerunsAsync(characterExerciseRerunFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<CharacterExerciseRerun> AddCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, CancellationToken cancellationToken = default)
    {
        return await AddCharacterExerciseRerunAsync(characterExerciseRerun, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<CharacterExerciseRerun> AddCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        characterExerciseRerun.Id = default(int);

        m_context.CharacterExerciseReruns.Entry(characterExerciseRerun).State = EntityState.Added;

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return characterExerciseRerun;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, CancellationToken cancellationToken = default)
    {
        return await UpdateCharacterExerciseRerunAsync(characterExerciseRerun, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> UpdateCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        CharacterExerciseRerunFilter characterExerciseRerunFilter = CreateCharacterExerciseRerunFilter(new CharacterExerciseRerunId(characterExerciseRerun.Id));

        CharacterExerciseRerun? existingCharacterExerciseRerun = (await GetCharacterExerciseRerunsAsync(characterExerciseRerunFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterExerciseRerun is null)
            return false;

        existingCharacterExerciseRerun.CharacterExerciseId = characterExerciseRerun.CharacterExerciseId;

        existingCharacterExerciseRerun.RequiredChallengeCount = characterExerciseRerun.RequiredChallengeCount;
        existingCharacterExerciseRerun.ContinuousChallengeCount = characterExerciseRerun.ContinuousChallengeCount;
        existingCharacterExerciseRerun.TotalChallengeCount = characterExerciseRerun.TotalChallengeCount;
        existingCharacterExerciseRerun.InitiallyScheduledOn = characterExerciseRerun.InitiallyScheduledOn;
        existingCharacterExerciseRerun.RepeatedlyScheduledOn = characterExerciseRerun.RepeatedlyScheduledOn;

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterExerciseRerunAsync(CharacterExerciseRerunId characterExerciseRerunId, CancellationToken cancellationToken = default)
    {
        CharacterExerciseRerunFilter characterExerciseRerunFilter = CreateCharacterExerciseRerunFilter(characterExerciseRerunId);

        CharacterExerciseRerun? existingCharacterExerciseRerun = (await GetCharacterExerciseRerunsAsync(characterExerciseRerunFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterExerciseRerun is null)
            return false;

        bool removed = await RemoveCharacterExerciseRerunAsync(existingCharacterExerciseRerun, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, CancellationToken cancellationToken = default)
    {
        return await RemoveCharacterExerciseRerunAsync(characterExerciseRerun, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> RemoveCharacterExerciseRerunAsync(CharacterExerciseRerun characterExerciseRerun, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        m_context.CharacterExerciseReruns.Remove(characterExerciseRerun);

        int rowsRemoved = 0;
        if (saveChangesImmediately)
            rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return !saveChangesImmediately || rowsRemoved > 0;
    }

    protected static CharacterExerciseRerunFilter CreateCharacterExerciseRerunFilter(CharacterExerciseRerunId characterExerciseRerunId)
    {
        int characterExerciseRerunIdRaw = characterExerciseRerunId.Value;

        IQueryable<CharacterExerciseRerun> CustomFilter(IQueryable<CharacterExerciseRerun> characterExerciseReruns) =>
            characterExerciseReruns.Where(characterExerciseRerun => characterExerciseRerun.Id == characterExerciseRerunIdRaw);

        CharacterExerciseRerunFilter characterExerciseRerunFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterExerciseRerunFilter;
    }

    protected static CharacterExerciseRerunFilter CreateCharacterExerciseRerunFilter(IEnumerable<CharacterExerciseRerunId> characterExerciseRerunIds)
    {
        IEnumerable<int> characterExerciseRerunIdsRaw = characterExerciseRerunIds.Select(characterExerciseRerunId => characterExerciseRerunId.Value);

        IQueryable<CharacterExerciseRerun> CustomFilter(IQueryable<CharacterExerciseRerun> characterExerciseReruns) =>
            characterExerciseReruns.Where(characterExerciseRerun => characterExerciseRerunIdsRaw.Contains(characterExerciseRerun.Id));

        CharacterExerciseRerunFilter characterExerciseRerunFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterExerciseRerunFilter;
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<CharacterExercise?> characterExercises)
    {
        foreach (CharacterExercise? characterExercise in characterExercises)
        {
            if (characterExercise?.CharacterExerciseReruns.Count > 0)
                characterExercise.CharacterExerciseReruns = [];

            ;
        }
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<Character?> characters)
    {
        foreach (Character? character in characters)
        {
            if (character?.CharacterGroup?.Characters.Count > 0)
                character.CharacterGroup.Characters = [];

            ;
        }
    }
}
