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

public class CharacterRepository : ICharacterRepository
{
    private readonly MainDbContext m_context;
    private readonly TimeProvider m_timeProvider;

    public CharacterRepository(MainDbContext context, TimeProvider timeProvider)
    {
        m_context = context;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public virtual async Task<Character?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        CharacterFilter characterFilter = CreateCharacterFilter(characterId);

        Character? character = (await GetCharactersAsync(characterFilter, cancellationToken)).SingleOrDefault();

        return character;
    }

    /// <inheritdoc />
    public virtual async Task<Character[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default)
    {
        CharacterFilter characterFilter = CreateCharacterFilter(characterIds);

        Character[] characters = await GetCharactersAsync(characterFilter, asNoTracking: true, cancellationToken: cancellationToken);

        return characters;
    }

    /// <inheritdoc />
    public virtual async Task<Character[]> GetCharactersAsync(CharacterFilter characterFilter, CancellationToken cancellationToken = default)
    {
        return await GetCharactersAsync(characterFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<Character[]> GetCharactersAsync(CharacterFilter? characterFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Character> charactersQueryable = m_context.Characters;

        charactersQueryable = charactersQueryable.Include(character => character.CharacterGroup);

        charactersQueryable = charactersQueryable.Include(character => character.CharacterTags)
            .ThenInclude(characterTag => characterTag.Tag);

        if (characterFilter is not null)
            charactersQueryable = characterFilter.Filter(charactersQueryable);

        if (asNoTracking)
            charactersQueryable = charactersQueryable.AsNoTracking();

        charactersQueryable = charactersQueryable.AsSingleQuery();

        Character[] characters = await charactersQueryable.ToArrayAsync(cancellationToken);

        return characters;
    }

    /// <inheritdoc />
    public virtual async Task<Character[]> GetAllCharactersAsync(CancellationToken cancellationToken = default)
    {
        return await GetCharactersAsync(characterFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<Character> AddCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await AddCharacterAsync(character, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<Character> AddCharacterAsync(Character character, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        character.Id = default(int);
        foreach (CharacterTag characterTag in character.CharacterTags)
            characterTag.CharacterId = default(int);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        character.CreatedOn = character.CreatedOn != default(DateTimeOffset)
            ? character.CreatedOn
            : utcNow;
        character.UpdatedOn = character.UpdatedOn != default(DateTimeOffset)
            ? character.UpdatedOn
            : utcNow;

        m_context.Characters.Entry(character).State = EntityState.Added;

        foreach (CharacterTag characterTag in character.CharacterTags)
            m_context.CharacterTags.Entry(characterTag).State = EntityState.Added;

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return character;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await UpdateCharacterAsync(character, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> UpdateCharacterAsync(Character character, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        CharacterFilter characterFilter = CreateCharacterFilter(new CharacterId(character.Id));

        Character? existingCharacter = (await GetCharactersAsync(characterFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacter is null)
            return false;

        existingCharacter.CharacterGroupId = character.CharacterGroupId;

        existingCharacter.Symbol = character.Symbol;
        existingCharacter.Type = character.Type;
        existingCharacter.Pronunciation = character.Pronunciation;
        existingCharacter.Syllable = character.Syllable;
        existingCharacter.Onyomi = character.Onyomi;
        existingCharacter.Kunyomi = character.Kunyomi;
        existingCharacter.Meaning = character.Meaning;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingCharacter.UpdatedOn = character.UpdatedOn != default(DateTimeOffset)
            ? character.UpdatedOn
            : utcNow;

        MergeCharacterTags(existingCharacter.CharacterTags, character.CharacterTags);

        if (saveChangesImmediately)
            await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        CharacterFilter characterFilter = CreateCharacterFilter(characterId);

        Character? existingCharacter = (await GetCharactersAsync(characterFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingCharacter is null)
            return false;

        bool removed = await RemoveCharacterAsync(existingCharacter, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await RemoveCharacterAsync(character, saveChangesImmediately: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> RemoveCharacterAsync(Character character, bool saveChangesImmediately = true, CancellationToken cancellationToken = default)
    {
        m_context.Characters.Remove(character);

        int rowsRemoved = 0;
        if (saveChangesImmediately)
            rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return !saveChangesImmediately || rowsRemoved > 0;
    }

    protected void MergeCharacterTags(IEnumerable<CharacterTag> targetCharacterTags, IEnumerable<CharacterTag> sourceCharacterTags)
    {
        HashSet<(int CharacterId, int TagId)> targetCharacterTagIds = targetCharacterTags
            .Select(characterTag => (characterTag.CharacterId, characterTag.TagId))
            .ToHashSet();
        HashSet<(int CharacterId, int TagId)> sourceCharacterTagIds = sourceCharacterTags
            .Select(characterTag => (characterTag.CharacterId, characterTag.TagId))
            .ToHashSet();

        foreach (CharacterTag sourceCharacterTag in sourceCharacterTags)
        {
            // The 'when not matched by target' clause:
            if (!targetCharacterTagIds.Contains((sourceCharacterTag.CharacterId, sourceCharacterTag.TagId)))
                m_context.CharacterTags.Entry(sourceCharacterTag).State = EntityState.Added;

            // The CharacterTag entity does not have any fields except the primary key,
            // therefore no 'when matched' clause.
        }

        foreach (CharacterTag targetCharacterTag in targetCharacterTags)
        {
            // The 'when not matched by source' clause:
            if (!sourceCharacterTagIds.Contains((targetCharacterTag.CharacterId, targetCharacterTag.TagId)))
                m_context.CharacterTags.Entry(targetCharacterTag).State = EntityState.Deleted;

            ;
        }
    }

    protected static CharacterFilter CreateCharacterFilter(CharacterId characterId)
    {
        int characterIdRaw = characterId.Value;

        IQueryable<Character> CustomFilter(IQueryable<Character> characters) =>
            characters.Where(character => character.Id == characterIdRaw);

        CharacterFilter characterFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterFilter;
    }

    protected static CharacterFilter CreateCharacterFilter(IEnumerable<CharacterId> characterIds)
    {
        IEnumerable<int> characterIdsRaw = characterIds.Select(characterId => characterId.Value);

        IQueryable<Character> CustomFilter(IQueryable<Character> characters) =>
            characters.Where(character => characterIdsRaw.Contains(character.Id));

        CharacterFilter characterFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return characterFilter;
    }
}
