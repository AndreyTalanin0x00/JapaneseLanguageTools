using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Contracts.Services.Abstractions;

public interface ICharacterService
{
    public Task<CharacterModel?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default);

    public Task<CharacterModel[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default);

    public Task<CharacterModel[]> GetAllCharactersAsync(CancellationToken cancellationToken = default);

    public Task<CharacterModel> AddCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default);
}
