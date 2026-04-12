using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Data.Contexts;
using JapaneseLanguageTools.Data.Entities;
using JapaneseLanguageTools.Data.Repositories.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace JapaneseLanguageTools.Data.Repositories;

public class TagRepository : ITagRepository
{
    private readonly MainDbContext m_context;
    private readonly TimeProvider m_timeProvider;

    public TagRepository(MainDbContext context, TimeProvider timeProvider)
    {
        m_context = context;
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public virtual async Task<Tag?> GetTagAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(tagId);

        Tag? tag = (await GetTagsAsync(tagFilter, cancellationToken)).SingleOrDefault();

        return tag;
    }

    /// <inheritdoc />
    public virtual async Task<Tag?> GetTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(tagCaption);

        Tag? tag = (await GetTagsAsync(tagFilter, cancellationToken)).SingleOrDefault();

        return tag;
    }

    /// <inheritdoc />
    public virtual async Task<Tag[]> GetTagsAsync(IEnumerable<TagId> tagIds, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(tagIds);

        Tag[] tags = await GetTagsAsync(tagFilter, cancellationToken: cancellationToken);

        return tags;
    }

    /// <inheritdoc />
    public virtual async Task<Tag[]> GetTagsAsync(IEnumerable<TagCaption> tagCaptions, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(tagCaptions);

        Tag[] tags = await GetTagsAsync(tagFilter, cancellationToken: cancellationToken);

        return tags;
    }

    /// <inheritdoc />
    public virtual async Task<Tag[]> GetTagsAsync(TagFilter tagFilter, CancellationToken cancellationToken = default)
    {
        return await GetTagsAsync(tagFilter, asNoTracking: true, cancellationToken: cancellationToken);
    }

    public virtual async Task<Tag[]> GetTagsAsync(TagFilter? tagFilter, bool asNoTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> tagsQueryable = m_context.Tags;

        if (tagFilter is not null)
            tagsQueryable = tagFilter.Filter(tagsQueryable);

        if (asNoTracking)
            tagsQueryable = tagsQueryable.AsNoTracking();

        tagsQueryable = tagsQueryable.AsSingleQuery();

        Tag[] tags = await tagsQueryable.ToArrayAsync(cancellationToken);

        return tags;
    }

    /// <inheritdoc />
    public virtual async Task<Tag[]> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        return await GetTagsAsync(tagFilter: null, asNoTracking: true, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<Tag> AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        tag.Id = default(int);

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        tag.CreatedOn = tag.CreatedOn != default(DateTimeOffset)
            ? tag.CreatedOn
            : utcNow;
        tag.UpdatedOn = tag.UpdatedOn != default(DateTimeOffset)
            ? tag.UpdatedOn
            : utcNow;

        m_context.Tags.Add(tag);

        await m_context.SaveChangesAsync(cancellationToken);

        return tag;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(new TagId(tag.Id));

        Tag? existingTag = (await GetTagsAsync(tagFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingTag is null)
            return false;

        existingTag.Caption = tag.Caption;
        existingTag.PlaceholderMarker = tag.PlaceholderMarker;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
        existingTag.UpdatedOn = tag.UpdatedOn != default(DateTimeOffset)
            ? tag.UpdatedOn
            : utcNow;

        await m_context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveTagAsync(TagId tagId, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(tagId);

        Tag? existingTag = (await GetTagsAsync(tagFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingTag is null)
            return false;

        bool removed = await RemoveTagAsync(existingTag, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveTagAsync(TagCaption tagCaption, CancellationToken cancellationToken = default)
    {
        TagFilter tagFilter = CreateTagFilter(tagCaption);

        Tag? existingTag = (await GetTagsAsync(tagFilter, asNoTracking: false, cancellationToken: cancellationToken)).SingleOrDefault();

        if (existingTag is null)
            return false;

        bool removed = await RemoveTagAsync(existingTag, cancellationToken);

        return removed;
    }

    /// <inheritdoc />
    public virtual async Task<bool> RemoveTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        m_context.Tags.Remove(tag);

        int rowsRemoved = await m_context.SaveChangesAsync(cancellationToken);

        return rowsRemoved > 0;
    }

    protected static TagFilter CreateTagFilter(TagId tagId)
    {
        int tagIdRaw = tagId.Value;

        IQueryable<Tag> CustomFilter(IQueryable<Tag> tags) =>
            tags.Where(tag => tag.Id == tagIdRaw);

        TagFilter tagFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return tagFilter;
    }

    protected static TagFilter CreateTagFilter(TagCaption tagCaption)
    {
        string tagCaptionRaw = tagCaption.Value;

        IQueryable<Tag> CustomFilter(IQueryable<Tag> tags) =>
            tags.Where(tag => tag.Caption == tagCaptionRaw);

        TagFilter tagFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return tagFilter;
    }

    protected static TagFilter CreateTagFilter(IEnumerable<TagId> tagIds)
    {
        IEnumerable<int> tagIdsRaw = tagIds.Select(tagId => tagId.Value);

        IQueryable<Tag> CustomFilter(IQueryable<Tag> tags) =>
            tags.Where(tag => tagIdsRaw.Contains(tag.Id));

        TagFilter tagFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return tagFilter;
    }

    protected static TagFilter CreateTagFilter(IEnumerable<TagCaption> tagCaptions)
    {
        IEnumerable<string> tagCaptionsRaw = tagCaptions.Select(tagCaption => tagCaption.Value);

        IQueryable<Tag> CustomFilter(IQueryable<Tag> tags) =>
            tags.Where(tag => tagCaptionsRaw.Contains(tag.Caption));

        TagFilter tagFilter = new()
        {
            CustomFilter = CustomFilter,
        };

        return tagFilter;
    }
}
