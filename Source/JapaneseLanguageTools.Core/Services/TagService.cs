using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

namespace JapaneseLanguageTools.Core.Services;

public class TagService : ITagService
{
    private readonly IMapper m_mapper;
    private readonly ITagRepository m_tagRepository;
    private readonly TimeProvider m_timeProvider;

    public TagService(IMapper mapper, ITagRepository tagRepository, TimeProvider timeProvider)
    {
        m_mapper = mapper;
        m_tagRepository = tagRepository;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<TagModel?> GetTagAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        Tag? tag = await m_tagRepository.GetTagAsync(tagId, cancellationToken);

        TagModel? tagModel = m_mapper.Map<TagModel?>(tag);

        return tagModel;
    }

    /// <inheritdoc />
    public async Task<TagModel?> GetTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default)
    {
        Tag? tag = await m_tagRepository.GetTagAsync(tagCaption, cancellationToken);

        TagModel? tagModel = m_mapper.Map<TagModel?>(tag);

        return tagModel;
    }

    /// <inheritdoc />
    public async Task<TagModel[]> GetTagsAsync(IEnumerable<TagId> tagIds, CancellationToken cancellationToken = default)
    {
        Tag[] tags = await m_tagRepository.GetTagsAsync(tagIds, cancellationToken);
        if (tags.Length == 0)
            return [];

        TagModel[] tagModels = m_mapper.Map<TagModel[]>(tags);

        return tagModels;
    }

    /// <inheritdoc />
    public async Task<TagModel[]> GetTagsAsync(IEnumerable<TagCaption> tagCaptions, CancellationToken cancellationToken = default)
    {
        Tag[] tags = await m_tagRepository.GetTagsAsync(tagCaptions, cancellationToken);
        if (tags.Length == 0)
            return [];

        TagModel[] tagModels = m_mapper.Map<TagModel[]>(tags);

        return tagModels;
    }

    /// <inheritdoc />
    public async Task<TagModel[]> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        Tag[] tags = await m_tagRepository.GetAllTagsAsync(cancellationToken);
        if (tags.Length == 0)
            return [];

        TagModel[] tagModels = m_mapper.Map<TagModel[]>(tags);

        return tagModels;
    }

    /// <inheritdoc />
    public async Task<TagModel> AddTagAsync(TagModel tagModel, CancellationToken cancellationToken = default)
    {
        Tag tag = m_mapper.Map<Tag>(tagModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        tag.CreatedOn = utcNow;
        tag.UpdatedOn = utcNow;

        Tag addedTag = await m_tagRepository.AddTagAsync(tag, cancellationToken);

        TagModel addedTagModel = m_mapper.Map<TagModel>(addedTag);

        return addedTagModel;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTagAsync(TagModel tagModel, CancellationToken cancellationToken = default)
    {
        Tag tag = m_mapper.Map<Tag>(tagModel);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        tag.UpdatedOn = utcNow;

        bool updated = await m_tagRepository.UpdateTagAsync(tag, cancellationToken);

        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveTagAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        bool removed = await m_tagRepository.RemoveTagAsync(tagId, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default)
    {
        bool removed = await m_tagRepository.RemoveTagAsync(tagCaption, cancellationToken);

        return removed;
    }
}
