using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Data.Repositories.Abstractions;

public interface ICharacterRepository
{
    public Task<Character?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default);

    public Task<Character[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default);

    public Task<Character[]> GetCharactersAsync(CharacterFilter characterFilter, CancellationToken cancellationToken = default);

    public Task<Character[]> GetAllCharactersAsync(CancellationToken cancellationToken = default);

    public Task<Character> AddCharacterAsync(Character character, CancellationToken cancellationToken = default);

    public Task<bool> UpdateCharacterAsync(Character character, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default);

    public Task<bool> RemoveCharacterAsync(Character character, CancellationToken cancellationToken = default);
}
