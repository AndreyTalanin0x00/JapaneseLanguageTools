using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

namespace JapaneseLanguageTools.Data.Repositories;

public class ApplicationDictionaryRepository : IApplicationDictionaryRepository
{
    private readonly ICharacterRepository m_characterRepository;
    private readonly ICharacterGroupRepository m_characterGroupRepository;
    private readonly IWordRepository m_wordRepository;
    private readonly IWordGroupRepository m_wordGroupRepository;

    public ApplicationDictionaryRepository(ICharacterRepository characterRepository, ICharacterGroupRepository characterGroupRepository, IWordRepository wordRepository, IWordGroupRepository wordGroupRepository)
    {
        m_characterRepository = characterRepository;
        m_characterGroupRepository = characterGroupRepository;
        m_wordRepository = wordRepository;
        m_wordGroupRepository = wordGroupRepository;
    }

    #region ICharacterRepository Repository Members

    /// <inheritdoc />
    public async Task<Character?> GetCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.GetCharacterAsync(characterId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Character[]> GetCharactersAsync(IEnumerable<CharacterId> characterIds, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.GetCharactersAsync(characterIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Character[]> GetCharactersAsync(CharacterFilter characterFilter, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.GetCharactersAsync(characterFilter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Character[]> GetAllCharactersAsync(CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.GetAllCharactersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Character> AddCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.AddCharacterAsync(character, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.UpdateCharacterAsync(character, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterAsync(CharacterId characterId, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.RemoveCharacterAsync(characterId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterAsync(Character character, CancellationToken cancellationToken = default)
    {
        return await m_characterRepository.RemoveCharacterAsync(character, cancellationToken);
    }

    #endregion

    #region ICharacterGroupRepository Repository Members

    /// <inheritdoc />
    public async Task<CharacterGroup?> GetCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.GetCharacterGroupAsync(characterGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroup?> GetCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.GetCharacterGroupAsync(characterGroupCaption, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupId> characterGroupIds, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.GetCharacterGroupsAsync(characterGroupIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroup[]> GetCharacterGroupsAsync(IEnumerable<CharacterGroupCaption> characterGroupCaptions, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.GetCharacterGroupsAsync(characterGroupCaptions, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroup[]> GetCharacterGroupsAsync(CharacterGroupFilter characterGroupFilter, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.GetCharacterGroupsAsync(characterGroupFilter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroup[]> GetAllCharacterGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.GetAllCharacterGroupsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CharacterGroup> AddCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.AddCharacterGroupAsync(characterGroup, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.UpdateCharacterGroupAsync(characterGroup, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ChangeCharacterGroupStateAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.ChangeCharacterGroupStateAsync(characterGroup, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroupId characterGroupId, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.RemoveCharacterGroupAsync(characterGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroupCaption characterGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.RemoveCharacterGroupAsync(characterGroupCaption, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCharacterGroupAsync(CharacterGroup characterGroup, CancellationToken cancellationToken = default)
    {
        return await m_characterGroupRepository.RemoveCharacterGroupAsync(characterGroup, cancellationToken);
    }

    #endregion

    #region IWordRepository Repository Members

    /// <inheritdoc />
    public async Task<Word?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.GetWordAsync(wordId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Word[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.GetWordsAsync(wordIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Word[]> GetWordsAsync(WordFilter wordFilter, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.GetWordsAsync(wordFilter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Word[]> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.GetAllWordsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Word> AddWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.AddWordAsync(word, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.UpdateWordAsync(word, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.RemoveWordAsync(wordId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordAsync(Word word, CancellationToken cancellationToken = default)
    {
        return await m_wordRepository.RemoveWordAsync(word, cancellationToken);
    }

    #endregion

    #region IWordGroupRepository Repository Members

    /// <inheritdoc />
    public async Task<WordGroup?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.GetWordGroupAsync(wordGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroup?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.GetWordGroupAsync(wordGroupCaption, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.GetWordGroupsAsync(wordGroupIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroup[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.GetWordGroupsAsync(wordGroupCaptions, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroup[]> GetWordGroupsAsync(WordGroupFilter wordGroupFilter, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.GetWordGroupsAsync(wordGroupFilter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroup[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.GetAllWordGroupsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WordGroup> AddWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.AddWordGroupAsync(wordGroup, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.UpdateWordGroupAsync(wordGroup, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ChangeWordGroupStateAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.ChangeWordGroupStateAsync(wordGroup, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.RemoveWordGroupAsync(wordGroupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.RemoveWordGroupAsync(wordGroupCaption, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroup wordGroup, CancellationToken cancellationToken = default)
    {
        return await m_wordGroupRepository.RemoveWordGroupAsync(wordGroup, cancellationToken);
    }

    #endregion
}
