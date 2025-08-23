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

public class CharacterGroupService : ICharacterGroupService
{
    private readonly IMapper m_mapper;
    private readonly ICharacterGroupRepository m_characterGroupRepository;
    private readonly TimeProvider m_timeProvider;

    public CharacterGroupService(IMapper mapper, ICharacterGroupRepository characterGroupRepository, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_characterGroupRepository = characterGroupRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        CharacterGroupId[] characterGroupIds = [characterGroupId];

        CharacterGroupModel? characterGroupModel = (await GetCharacterGroupsAsync(characterGroupIds, cancellationToken)).SingleOrDefault();

        return characterGroupModel;
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        CharacterGroupCaption[] characterGroupCaptions = [characterGroupCaption];

        CharacterGroupModel? characterGroupModel = (await GetCharacterGroupsAsync(characterGroupCaptions, cancellationToken)).SingleOrDefault();

        return characterGroupModel;
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default)
    {
        CharacterGroup[] characterGroups = await m_characterGroupRepository.GetCharacterGroupsAsync(characterGroupIds, cancellationToken);
        if (characterGroups.Length == 0)
            return [];

        CharacterGroupModel[] characterGroupModels = m_mapper.Map<CharacterGroupModel[]>(characterGroups);

        RemoveNavigationPropertyCycles(characterGroupModels);

        return characterGroupModels;
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default)
    {
        CharacterGroup[] characterGroups = await m_characterGroupRepository.GetCharacterGroupsAsync(characterGroupCaptions, cancellationToken);
        if (characterGroups.Length == 0)
            return [];

        CharacterGroupModel[] characterGroupModels = m_mapper.Map<CharacterGroupModel[]>(characterGroups);

        RemoveNavigationPropertyCycles(characterGroupModels);

        return characterGroupModels;
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default)
    {
        CharacterGroup[] characterGroups = await m_characterGroupRepository.GetAllCharacterGroupsAsync(cancellationToken);
        if (characterGroups.Length == 0)
            return [];

        CharacterGroupModel[] characterGroupModels = m_mapper.Map<CharacterGroupModel[]>(characterGroups);

        RemoveNavigationPropertyCycles(characterGroupModels);

        return characterGroupModels;
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel> AddCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        CharacterGroup characterGroup = m_mapper.Map<CharacterGroup>(characterGroupModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        characterGroup.CreatedOn = utcNow;
        characterGroup.UpdatedOn = utcNow;

        CharacterGroup addedCharacterGroup = await m_characterGroupRepository.AddCharacterGroupAsync(characterGroup, cancellationToken);

        CharacterGroupModel addedCharacterGroupModel = m_mapper.Map<CharacterGroupModel>(addedCharacterGroup);

        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedCharacterGroupModel));

        return addedCharacterGroupModel;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        CharacterGroup characterGroup = m_mapper.Map<CharacterGroup>(characterGroupModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        characterGroup.UpdatedOn = utcNow;

        bool updated = await m_characterGroupRepository.UpdateCharacterGroupAsync(characterGroup, cancellationToken);

        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> ChangeCharacterGroupStateAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        CharacterGroup characterGroup = m_mapper.Map<CharacterGroup>(characterGroupModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        characterGroup.UpdatedOn = utcNow;

        bool stateChanged = await m_characterGroupRepository.ChangeCharacterGroupStateAsync(characterGroup, cancellationToken);

        return stateChanged;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_characterGroupRepository.RemoveCharacterGroupAsync(characterGroupId, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        bool removed = await m_characterGroupRepository.RemoveCharacterGroupAsync(characterGroupCaption, cancellationToken);

        return removed;
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<CharacterGroupModel> characterGroupModels)
    {
        foreach (CharacterGroupModel characterGroupModel in characterGroupModels)
        {
            foreach (CharacterModel characterModel in characterGroupModel.Characters)
            {
                characterModel.CharacterGroup = null;
            }
        }
    }
}
