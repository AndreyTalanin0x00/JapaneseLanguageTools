using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.YieldHelpers;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class CharacterService : ICharacterService
{
    private readonly IMapper m_mapper;
    private readonly ICharacterRepository m_characterRepository;
    private readonly TimeProvider m_timeProvider;

    public CharacterService(IMapper mapper, ICharacterRepository characterRepository, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_characterRepository = characterRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<CharacterModel?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        CharacterId[] characterIds = [characterId];

        CharacterModel? characterModel = (await GetCharactersAsync(characterIds, cancellationToken)).SingleOrDefault();

        return characterModel;
    }

    /// <inheritdoc />
    public async Task<CharacterModel[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default)
    {
        Character[] characters = await m_characterRepository.GetCharactersAsync(characterIds, cancellationToken);
        if (characters.Length == 0)
            return [];

        CharacterModel[] characterModels = m_mapper.Map<CharacterModel[]>(characters);

        RemoveNavigationPropertyCycles(characterModels);

        return characterModels;
    }

    /// <inheritdoc />
    public async Task<CharacterModel[]> GetAllCharactersAsync(CancellationToken cancellationToken = default)
    {
        Character[] characters = await m_characterRepository.GetAllCharactersAsync(cancellationToken);
        if (characters.Length == 0)
            return [];

        CharacterModel[] characterModels = m_mapper.Map<CharacterModel[]>(characters);

        RemoveNavigationPropertyCycles(characterModels);

        return characterModels;
    }

    /// <inheritdoc />
    public async Task<CharacterModel> AddCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default)
    {
        Character character = m_mapper.Map<Character>(characterModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        character.CreatedOn = utcNow;
        character.UpdatedOn = utcNow;

        Character addedCharacter = await m_characterRepository.AddCharacterAsync(character, cancellationToken);

        CharacterModel addedCharacterModel = m_mapper.Map<CharacterModel>(addedCharacter);

        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedCharacterModel));

        return addedCharacterModel;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default)
    {
        Character character = m_mapper.Map<Character>(characterModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        character.UpdatedOn = utcNow;

        bool updated = await m_characterRepository.UpdateCharacterAsync(character, cancellationToken);

        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_characterRepository.RemoveCharacterAsync(characterId, cancellationToken);

        return removed;
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<CharacterModel> characterModels)
    {
        foreach (CharacterModel characterModel in characterModels)
        {
            if (characterModel.CharacterGroup?.Characters.Count > 0)
            {
                characterModel.CharacterGroup.Characters = [];
            }
        }
    }
}
