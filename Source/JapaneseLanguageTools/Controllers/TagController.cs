using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JapaneseLanguageTools.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TagController : ControllerBase
{
    private readonly ITagService m_tagService;
    private readonly ILogger<TagController> m_logger;

    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        m_tagService = tagService;
        m_logger = logger;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TagModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagModel>> GetTagAsync([Required][FromQuery] int tagId, CancellationToken cancellationToken)
    {
        TagId tagIdWrapped = new(tagId);

        TagModel? tagModel = await m_tagService.GetTagAsync(tagIdWrapped, cancellationToken);

        ActionResult<TagModel> actionResult = tagModel is not null
            ? (ActionResult<TagModel>)Ok(tagModel)
            : (ActionResult<TagModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TagModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagModel>> GetTagByCaptionAsync([Required][FromQuery] string tagCaption, CancellationToken cancellationToken)
    {
        TagCaption tagCaptionWrapped = new(tagCaption);

        TagModel? tagModel = await m_tagService.GetTagAsync(tagCaptionWrapped, cancellationToken);

        ActionResult<TagModel> actionResult = tagModel is not null
            ? (ActionResult<TagModel>)Ok(tagModel)
            : (ActionResult<TagModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TagModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TagModel[]>> GetTagsAsync([Required][FromQuery] int[] tagIds, CancellationToken cancellationToken)
    {
        IEnumerable<TagId> tagIdsWrapped =
            tagIds.Select(tagId => new TagId(tagId));

        TagModel[] tagModels = await m_tagService.GetTagsAsync(tagIdsWrapped, cancellationToken);

        return Ok(tagModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TagModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TagModel[]>> GetTagsByCaptionAsync([Required][FromQuery] string[] tagCaptions, CancellationToken cancellationToken)
    {
        IEnumerable<TagCaption> tagCaptionsWrapped =
            tagCaptions.Select(tagCaption => new TagCaption(tagCaption));

        TagModel[] tagModels = await m_tagService.GetTagsAsync(tagCaptionsWrapped, cancellationToken);

        return Ok(tagModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TagModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TagModel[]>> GetAllTagsAsync(CancellationToken cancellationToken)
    {
        TagModel[] tagModels = await m_tagService.GetAllTagsAsync(cancellationToken);

        return Ok(tagModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TagModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TagModel>> AddTagAsync([Required][FromBody] TagModel tagModel, CancellationToken cancellationToken)
    {
        try
        {
            TagModel addedTagModel = await m_tagService.AddTagAsync(tagModel, cancellationToken);

            return Ok(addedTagModel);
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to add the tag.");

            return Conflict();
        }
    }

    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateTagAsync([Required][FromBody] TagModel tagModel, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await m_tagService.UpdateTagAsync(tagModel, cancellationToken);

            return updated ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to update the tag.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveTagAsync([Required][FromQuery] int tagId, CancellationToken cancellationToken)
    {
        try
        {
            TagId tagIdWrapped = new(tagId);

            bool removed = await m_tagService.RemoveTagAsync(tagIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the tag.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveTagByCaptionAsync([Required][FromQuery] string tagCaption, CancellationToken cancellationToken)
    {
        try
        {
            TagCaption tagCaptionWrapped = new(tagCaption);

            bool removed = await m_tagService.RemoveTagAsync(tagCaptionWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the tag.");

            return Conflict();
        }
    }
}
