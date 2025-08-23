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

public class WordService : IWordService
{
    private readonly IMapper m_mapper;
    private readonly IWordRepository m_wordRepository;
    private readonly TimeProvider m_timeProvider;

    public WordService(IMapper mapper, IWordRepository wordRepository, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_wordRepository = wordRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<WordModel?> GetWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        WordId[] wordIds = [wordId];

        WordModel? wordModel = (await GetWordsAsync(wordIds, cancellationToken)).SingleOrDefault();

        return wordModel;
    }

    /// <inheritdoc />
    public async Task<WordModel[]> GetWordsAsync(IEnumerable<WordId> wordIds, CancellationToken cancellationToken = default)
    {
        Word[] words = await m_wordRepository.GetWordsAsync(wordIds, cancellationToken);
        if (words.Length == 0)
            return [];

        WordModel[] wordModels = m_mapper.Map<WordModel[]>(words);

        RemoveNavigationPropertyCycles(wordModels);

        return wordModels;
    }

    /// <inheritdoc />
    public async Task<WordModel[]> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        Word[] words = await m_wordRepository.GetAllWordsAsync(cancellationToken);
        if (words.Length == 0)
            return [];

        WordModel[] wordModels = m_mapper.Map<WordModel[]>(words);

        RemoveNavigationPropertyCycles(wordModels);

        return wordModels;
    }

    /// <inheritdoc />
    public async Task<WordModel> AddWordAsync(WordModel wordModel, CancellationToken cancellationToken = default)
    {
        Word word = m_mapper.Map<Word>(wordModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        word.CreatedOn = utcNow;
        word.UpdatedOn = utcNow;

        Word addedWord = await m_wordRepository.AddWordAsync(word, cancellationToken);

        WordModel addedWordModel = m_mapper.Map<WordModel>(addedWord);

        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedWordModel));

        return addedWordModel;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWordAsync(WordModel wordModel, CancellationToken cancellationToken = default)
    {
        Word word = m_mapper.Map<Word>(wordModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        word.UpdatedOn = utcNow;

        bool updated = await m_wordRepository.UpdateWordAsync(word, cancellationToken);

        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordAsync(WordId wordId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_wordRepository.RemoveWordAsync(wordId, cancellationToken);

        return removed;
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<WordModel> wordModels)
    {
        foreach (WordModel wordModel in wordModels)
        {
            if (wordModel.WordGroup?.Words.Count > 0)
            {
                wordModel.WordGroup.Words = [];
            }
        }
    }
}
