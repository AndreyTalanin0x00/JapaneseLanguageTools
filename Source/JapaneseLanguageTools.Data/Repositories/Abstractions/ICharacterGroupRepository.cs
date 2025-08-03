using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface ICharacterGroupRepository
{
    public Task<CharacterGroup?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default);

    public Task<CharacterGroup?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default);

    public Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default);

    public Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default);

    public Task<CharacterGroup[]> GetCharacterGroupsAsync(CharacterGroupFilter characterGroupFilter, CancellationToken cancellationToken = default);

    public Task<CharacterGroup[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default);

    public Task<CharacterGroup> AddCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default);

    public Task<bool> ChangeCharacterGroupStateAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default);
}
