using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class CharacterService : ICharacterService
{
    public CharacterService()
    {
        throw new NotImplementedException();
    }

    public Task<CharacterModel?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterModel[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterModel[]> GetAllCharactersAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CharacterModel> AddCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
