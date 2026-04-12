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

public class CharacterGroupRepository : ICharacterGroupRepository
{
    private readonly MainDbContext m_context;
    private readonly CharacterRepository m_characterRepository;
    private readonly TimeProvider m_timeProvider;

    public CharacterGroupRepository(MainDbContext context, CharacterRepository characterRepository, TimeProvider timeProvider)
    {
        m_context = context;
        m_characterRepository = characterRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(characterGroupId);

        CharacterGroup? characterGroup = (await GetCharacterGroupsAsync(characterGroupFilter, cancellationToken)).SingleOrDefault();

        return characterGroup;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(characterGroupCaption);

        CharacterGroup? characterGroup = (await GetCharacterGroupsAsync(characterGroupFilter, cancellationToken)).SingleOrDefault();

        return characterGroup;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(characterGroupIds);

        CharacterGroup[] characterGroups = await GetCharacterGroupsAsync(characterGroupFilter, cancellationToken: cancellationToken);

        return characterGroups;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(characterGroupCaptions);

        CharacterGroup[] characterGroups = await GetCharacterGroupsAsync(characterGroupFilter, cancellationToken: cancellationToken);

        return characterGroups;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup[]> GetCharacterGroupsAsync(CharacterGroupFilter characterGroupFilter, CancellationToken cancellationToken = default)
    {
        return await GetCharacterGroupsAsync(characterGroupFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<CharacterGroup[]> GetCharacterGroupsAsync(CharacterGroupFilter? characterGroupFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<CharacterGroup> characterGroupsQueryable = m_context.CharacterGroups;

        characterGroupsQueryable = characterGroupsQueryable.Include(characterGroup => characterGroup.Characters)
            .ThenInclude(character => character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);

        if (characterGroupFilter is not null)
            characterGroupsQueryable = characterGroupFilter.Filter(characterGroupsQueryable);

        if (asNoTracking)
            characterGroupsQueryable = characterGroupsQueryable.AsNoTracking();

        characterGroupsQueryable = characterGroupsQueryable.AsSingleQuery();

        CharacterGroup[] characterGroups = await characterGroupsQueryable.ToArrayAsync(cancellationToken);

        return characterGroups;
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await GetCharacterGroupsAsync(characterGroupFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<CharacterGroup> AddCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        characterGroup.Id = default(int);
        foreach (Character character in characterGroup.Characters)
            character.CharacterGroupId = default(int);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        characterGroup.CreatedOn = characterGroup.CreatedOn != default(DateTimeOffset)
            ? characterGroup.CreatedOn
            : utcNow;
        characterGroup.UpdatedOn = characterGroup.UpdatedOn != default(DateTimeOffset)
            ? characterGroup.UpdatedOn
            : utcNow;

        m_context.CharacterGroups.Entry(characterGroup).State = EntityState.Added;

        foreach (Character character in characterGroup.Characters)
            await m_characterRepository.AddCharacterAsync(character, saveChangesImmediately: false, cancellationToken: cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return characterGroup;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(new CharacterGroupId(characterGroup.Id));

        CharacterGroup? existingCharacterGroup = (await GetCharacterGroupsAsync(characterGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterGroup is null)
            return false;

        existingCharacterGroup.Caption = characterGroup.Caption;
        existingCharacterGroup.Comment = characterGroup.Comment;
        existingCharacterGroup.Enabled = characterGroup.Enabled;
        existingCharacterGroup.AlwaysUse = characterGroup.AlwaysUse;
        existingCharacterGroup.Hidden = characterGroup.Hidden;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingCharacterGroup.UpdatedOn = characterGroup.UpdatedOn != default(DateTimeOffset)
            ? characterGroup.UpdatedOn
            : utcNow;

        await MergeCharactersAsync(existingCharacterGroup.Characters, characterGroup.Characters, cancellationToken);

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> ChangeCharacterGroupStateAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(new CharacterGroupId(characterGroup.Id));

        CharacterGroup? existingCharacterGroup = (await GetCharacterGroupsAsync(characterGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterGroup is null)
            return false;

        existingCharacterGroup.Enabled = characterGroup.Enabled;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingCharacterGroup.UpdatedOn = characterGroup.UpdatedOn != default(DateTimeOffset)
            ? characterGroup.UpdatedOn
            : utcNow;

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(characterGroupId);

        CharacterGroup? existingCharacterGroup = (await GetCharacterGroupsAsync(characterGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterGroup is null)
            return false;

        bool removed = await RemoveCharacterGroupAsync(existingCharacterGroup, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        CharacterGroupFilter characterGroupFilter = CreateCharacterGroupFilter(characterGroupCaption);

        CharacterGroup? existingCharacterGroup = (await GetCharacterGroupsAsync(characterGroupFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacterGroup is null)
            return false;

        bool removed = await RemoveCharacterGroupAsync(existingCharacterGroup, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        foreach (Character character in characterGroup.Characters)
            await m_characterRepository.RemoveCharacterAsync(character, saveChangesImmediately: false, cancellationToken: cancellationToken);

        m_context.CharacterGroups.Remove(characterGroup);

        int rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return rowsRemoved > 0;
    }

    protected async Task MergeCharactersAsync(IEnumerable<Character> targetCharacters, IEnumerable<Character> sourceCharacters, CancellationToken cancellationToken = default)
    {
        HashSet<int> targetCharacterIds = targetCharacters
            .Select(character => character.Id)
            .ToHashSet();
        HashSet<int> sourceCharacterIds = sourceCharacters
            .Select(character => character.Id)
            .ToHashSet();

        foreach (Character sourceCharacter in sourceCharacters)
        {
            // The 'when not matched by target' clause:
            if (!targetCharacterIds.Contains(sourceCharacter.Id))
            {
                await m_characterRepository.AddCharacterAsync(sourceCharacter, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
            // The 'when matched' clause:
            else // if (targetCharacterIds.Contains(sourceCharacter.Id))
            {
                await m_characterRepository.UpdateCharacterAsync(sourceCharacter, saveChangesImmediately: false, cancellationToken: cancellationToken);
            }
        }

        foreach (Character targetCharacter in targetCharacters)
        {
            // The 'when not matched by source' clause:
            if (!sourceCharacterIds.Contains(targetCharacter.Id))
                await m_characterRepository.RemoveCharacterAsync(targetCharacter, saveChangesImmediately: false, cancellationToken: cancellationToken);

            ;
        }
    }

    protected static CharacterGroupFilter CreateCharacterGroupFilter(CharacterGroupId characterGroupId)
    {
        int characterGroupIdRaw = characterGroupId.Value;

        IQueryable<CharacterGroup> CustomFilter(IQueryable<CharacterGroup> characterGroups) =>
            characterGroups.Where(characterGroup => characterGroup.Id == characterGroupIdRaw);

        CharacterGroupFilter characterGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterGroupFilter;
    }

    protected static CharacterGroupFilter CreateCharacterGroupFilter(CharacterGroupCaption characterGroupCaption)
    {
        string characterGroupCaptionRaw = characterGroupCaption.Value;

        IQueryable<CharacterGroup> CustomFilter(IQueryable<CharacterGroup> characterGroups) =>
            characterGroups.Where(characterGroup => characterGroup.Caption == characterGroupCaptionRaw);

        CharacterGroupFilter characterGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterGroupFilter;
    }

    protected static CharacterGroupFilter CreateCharacterGroupFilter(IEnumerable<CharacterGroupId> characterGroupIds)
    {
        IEnumerable<int> characterGroupIdsRaw = characterGroupIds.Select(characterGroupId => characterGroupId.Value);

        IQueryable<CharacterGroup> CustomFilter(IQueryable<CharacterGroup> characterGroups) =>
            characterGroups.Where(characterGroup => characterGroupIdsRaw.Contains(characterGroup.Id));

        CharacterGroupFilter characterGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterGroupFilter;
    }

    protected static CharacterGroupFilter CreateCharacterGroupFilter(IEnumerable<CharacterGroupCaption> characterGroupCaptions)
    {
        IEnumerable<string> characterGroupCaptionsRaw = characterGroupCaptions.Select(characterGroupCaption => characterGroupCaption.Value);

        IQueryable<CharacterGroup> CustomFilter(IQueryable<CharacterGroup> characterGroups) =>
            characterGroups.Where(characterGroup => characterGroupCaptionsRaw.Contains(characterGroup.Caption));

        CharacterGroupFilter characterGroupFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterGroupFilter;
    }
}
