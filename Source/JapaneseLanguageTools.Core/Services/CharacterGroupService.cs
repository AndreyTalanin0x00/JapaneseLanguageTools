using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class CharacterGroupService : ICharacterGroupService
{
    public CharacterGroupService()
    {
        throw new NotImplementedException();
    }

    public Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterGroupModel[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterGroupModel> AddCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangeCharacterGroupStateAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
