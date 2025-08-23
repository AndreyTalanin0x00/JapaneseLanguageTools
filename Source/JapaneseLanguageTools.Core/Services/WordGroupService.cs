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

public class WordGroupService : IWordGroupService
{
    private readonly IMapper m_mapper;
    private readonly IWordGroupRepository m_wordGroupRepository;
    private readonly TimeProvider m_timeProvider;

    public WordGroupService(IMapper mapper, IWordGroupRepository wordGroupRepository, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_wordGroupRepository = wordGroupRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<WordGroupModel?> GetWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        WordGroupId[] wordGroupIds = [wordGroupId];

        WordGroupModel? wordGroupModel = (await GetWordGroupsAsync(wordGroupIds, cancellationToken)).SingleOrDefault();

        return wordGroupModel;
    }

    /// <inheritdoc />
    public async Task<WordGroupModel?> GetWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        WordGroupCaption[] wordGroupCaptions = [wordGroupCaption];

        WordGroupModel? wordGroupModel = (await GetWordGroupsAsync(wordGroupCaptions, cancellationToken)).SingleOrDefault();

        return wordGroupModel;
    }

    /// <inheritdoc />
    public async Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupId> wordGroupIds, CancellationToken cancellationToken = default)
    {
        WordGroup[] wordGroups = await m_wordGroupRepository.GetWordGroupsAsync(wordGroupIds, cancellationToken);
        if (wordGroups.Length == 0)
            return [];

        WordGroupModel[] wordGroupModels = m_mapper.Map<WordGroupModel[]>(wordGroups);

        RemoveNavigationPropertyCycles(wordGroupModels);

        return wordGroupModels;
    }

    /// <inheritdoc />
    public async Task<WordGroupModel[]> GetWordGroupsAsync(IEnumerable<WordGroupCaption> wordGroupCaptions, CancellationToken cancellationToken = default)
    {
        WordGroup[] wordGroups = await m_wordGroupRepository.GetWordGroupsAsync(wordGroupCaptions, cancellationToken);
        if (wordGroups.Length == 0)
            return [];

        WordGroupModel[] wordGroupModels = m_mapper.Map<WordGroupModel[]>(wordGroups);

        RemoveNavigationPropertyCycles(wordGroupModels);

        return wordGroupModels;
    }

    /// <inheritdoc />
    public async Task<WordGroupModel[]> GetAllWordGroupsAsync(CancellationToken cancellationToken = default)
    {
        WordGroup[] wordGroups = await m_wordGroupRepository.GetAllWordGroupsAsync(cancellationToken);
        if (wordGroups.Length == 0)
            return [];

        WordGroupModel[] wordGroupModels = m_mapper.Map<WordGroupModel[]>(wordGroups);

        RemoveNavigationPropertyCycles(wordGroupModels);

        return wordGroupModels;
    }

    /// <inheritdoc />
    public async Task<WordGroupModel> AddWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        WordGroup wordGroup = m_mapper.Map<WordGroup>(wordGroupModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        wordGroup.CreatedOn = utcNow;
        wordGroup.UpdatedOn = utcNow;

        WordGroup addedWordGroup = await m_wordGroupRepository.AddWordGroupAsync(wordGroup, cancellationToken);

        WordGroupModel addedWordGroupModel = m_mapper.Map<WordGroupModel>(addedWordGroup);

        RemoveNavigationPropertyCycles(YieldEnumerableHelpers.Yield(addedWordGroupModel));

        return addedWordGroupModel;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWordGroupAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        WordGroup wordGroup = m_mapper.Map<WordGroup>(wordGroupModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        wordGroup.UpdatedOn = utcNow;

        bool updated = await m_wordGroupRepository.UpdateWordGroupAsync(wordGroup, cancellationToken);

        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> ChangeWordGroupStateAsync(WordGroupModel wordGroupModel, CancellationToken cancellationToken = default)
    {
        WordGroup wordGroup = m_mapper.Map<WordGroup>(wordGroupModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        wordGroup.UpdatedOn = utcNow;

        bool stateChanged = await m_wordGroupRepository.ChangeWordGroupStateAsync(wordGroup, cancellationToken);

        return stateChanged;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroupId wordGroupId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_wordGroupRepository.RemoveWordGroupAsync(wordGroupId, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWordGroupAsync(WordGroupCaption wordGroupCaption, CancellationToken cancellationToken = default)
    {
        bool removed = await m_wordGroupRepository.RemoveWordGroupAsync(wordGroupCaption, cancellationToken);

        return removed;
    }

    protected static void RemoveNavigationPropertyCycles(IEnumerable<WordGroupModel> wordGroupModels)
    {
        foreach (WordGroupModel wordGroupModel in wordGroupModels)
        {
            foreach (WordModel wordModel in wordGroupModel.Words)
            {
                wordModel.WordGroup = null;
            }
        }
    }
}
