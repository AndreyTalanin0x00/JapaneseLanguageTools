using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface ICharacterGroupService
{
    public Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default);

    public Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default);

    public Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default);

    public Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default);

    public Task<CharacterGroupModel[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default);

    public Task<CharacterGroupModel> AddCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default);

    public Task<bool> ChangeCharacterGroupStateAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default);
}
