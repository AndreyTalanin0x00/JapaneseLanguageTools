using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class ApplicationDictionaryService : IApplicationDictionaryService
{
    private readonly ICharacterService m_characterService;
    private readonly ICharacterGroupService m_characterGroupService;
    private readonly IWordService m_wordService;
    private readonly IWordGroupService m_wordGroupService;

    public ApplicationDictionaryService(ICharacterService characterService, ICharacterGroupService characterGroupService, IWordService wordService, IWordGroupService wordGroupService)
    {
        m_characterService = characterService;
        m_characterGroupService = characterGroupService;
        m_wordService = wordService;
        m_wordGroupService = wordGroupService;
    }

    #region ICharacterService Service Members

    /// <inheritdoc />
    public async Task<CharacterModel?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        return await m_characterService.GetCharacterAsync(characterId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterModel[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default)
    {
        return await m_characterService.GetCharactersAsync(characterIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterModel[]> GetAllCharactersAsync(CancellationToken cancellationToken = default)
    {
        return await m_characterService.GetAllCharactersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterModel> AddCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default)
    {
        return await m_characterService.AddCharacterAsync(characterModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterAsync(CharacterModel characterModel, CancellationToken cancellationToken = default)
    {
        return await m_characterService.UpdateCharacterAsync(characterModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        return await m_characterService.RemoveCharacterAsync(characterId, cancellationToken);
    }

    #endregion

    #region ICharacterGroupService Service Members

    /// <inheritdoc />
    public async Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.GetCharacterGroupAsync(characterGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.GetCharacterGroupAsync(characterGroupCaption, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.GetCharacterGroupsAsync(characterGroupIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.GetCharacterGroupsAsync(characterGroupCaptions, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.GetAllCharacterGroupsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroupModel> AddCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.AddCharacterGroupAsync(characterGroupModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterGroupAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.UpdateCharacterGroupAsync(characterGroupModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ChangeCharacterGroupStateAsync(CharacterGroupModel characterGroupModel, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.ChangeCharacterGroupStateAsync(characterGroupModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.RemoveCharacterGroupAsync(characterGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupService.RemoveCharacterGroupAsync(characterGroupCaption, cancellationToken);
    }

    #endregion

    #region IWordService Service Members

    /// <inheritdoc />
    public async Task<WordModel?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        return await m_wordService.GetWordAsync(wordId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordModel[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default)
    {
        return await m_wordService.GetWordsAsync(wordIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordModel[]> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        return await m_wordService.GetAllWordsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordModel> AddWordAsync(WordModel wordModel, CancellationToken cancellationToken = default)
    {
        return await m_wordService.AddWordAsync(wordModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWordAsync(WordModel wordModel, CancellationToken cancellationToken = default)
    {
        return await m_wordService.UpdateWordAsync(wordModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        return await m_wordService.RemoveWordAsync(wordId, cancellationToken);
    }

    #endregion

    #region IWordGroupService Service Members

    /// <inheritdoc />
    public async Task<WordGroupModel?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.GetWordGroupAsync(wordGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroupModel?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.GetWordGroupAsync(wordGroupCaption, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.GetWordGroupsAsync(wordGroupIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.GetWordGroupsAsync(wordGroupCaptions, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroupModel[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.GetAllWordGroupsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroupModel> AddWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.AddWordGroupAsync(wordGroupModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.UpdateWordGroupAsync(wordGroupModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ChangeWordGroupStateAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.ChangeWordGroupStateAsync(wordGroupModel, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.RemoveWordGroupAsync(wordGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupService.RemoveWordGroupAsync(wordGroupCaption, cancellationToken);
    }

    #endregion
}
