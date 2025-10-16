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
public class ApplicationDictionaryController : ControllerBase
{
    private readonly IApplicationDictionaryService m_applicationDictionaryService;
    private readonly ILogger<ApplicationDictionaryController> m_logger;

    public ApplicationDictionaryController(IApplicationDictionaryService applicationDictionaryService, ILogger<ApplicationDictionaryController> logger)
    {
        m_applicationDictionaryService = applicationDictionaryService;
        m_logger = logger;
    }

    #region ICharacterService Service Members

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterModel>> GetCharacterAsync([Required][FromQuery] int characterId, CancellationToken cancellationToken)
    {
        CharacterId characterIdWrapped = new(characterId);

        CharacterModel? characterModel = await m_applicationDictionaryService.GetCharacterAsync(characterIdWrapped, cancellationToken);

        ActionResult<CharacterModel> actionResult = characterModel is not null
            ? (ActionResult<CharacterModel>)Ok(characterModel)
            : (ActionResult<CharacterModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterModel[]>> GetCharactersAsync([Required][FromQuery] int[] characterIds, CancellationToken cancellationToken)
    {
        IEnumerable<CharacterId> characterIdsWrapped =
            characterIds.Select(characterId => new CharacterId(characterId));

        CharacterModel[] characterModels = await m_applicationDictionaryService.GetCharactersAsync(characterIdsWrapped, cancellationToken);

        return Ok(characterModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterModel[]>> GetAllCharactersAsync(CancellationToken cancellationToken)
    {
        CharacterModel[] characterModels = await m_applicationDictionaryService.GetAllCharactersAsync(cancellationToken);

        return Ok(characterModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CharacterModel>> AddCharacterAsync([Required][FromBody] CharacterModel characterModel, CancellationToken cancellationToken)
    {
        try
        {
            CharacterModel addedCharacterModel = await m_applicationDictionaryService.AddCharacterAsync(characterModel, cancellationToken);

            return Ok(addedCharacterModel);
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to add the character.");

            return Conflict();
        }
    }

    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateCharacterAsync([Required][FromBody] CharacterModel characterModel, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await m_applicationDictionaryService.UpdateCharacterAsync(characterModel, cancellationToken);

            return updated ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to update the character.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveCharacterAsync([Required][FromQuery] int characterId, CancellationToken cancellationToken)
    {
        try
        {
            CharacterId characterIdWrapped = new(characterId);

            bool removed = await m_applicationDictionaryService.RemoveCharacterAsync(characterIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the character.");

            return Conflict();
        }
    }

    #endregion

    #region ICharacterGroupService Service Members

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterGroupModel>> GetCharacterGroupAsync([Required][FromQuery] int characterGroupId, CancellationToken cancellationToken)
    {
        CharacterGroupId characterGroupIdWrapped = new(characterGroupId);

        CharacterGroupModel? characterGroupModel = await m_applicationDictionaryService.GetCharacterGroupAsync(characterGroupIdWrapped, cancellationToken);

        ActionResult<CharacterGroupModel> actionResult = characterGroupModel is not null
            ? (ActionResult<CharacterGroupModel>)Ok(characterGroupModel)
            : (ActionResult<CharacterGroupModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterGroupModel>> GetCharacterGroupByCaptionAsync([Required][FromQuery] string characterGroupCaption, CancellationToken cancellationToken)
    {
        CharacterGroupCaption characterGroupCaptionWrapped = new(characterGroupCaption);

        CharacterGroupModel? characterGroupModel = await m_applicationDictionaryService.GetCharacterGroupAsync(characterGroupCaptionWrapped, cancellationToken);

        ActionResult<CharacterGroupModel> actionResult = characterGroupModel is not null
            ? (ActionResult<CharacterGroupModel>)Ok(characterGroupModel)
            : (ActionResult<CharacterGroupModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterGroupModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterGroupModel[]>> GetCharacterGroupsAsync([Required][FromQuery] int[] characterGroupIds, CancellationToken cancellationToken)
    {
        IEnumerable<CharacterGroupId> characterGroupIdsWrapped =
            characterGroupIds.Select(characterGroupId => new CharacterGroupId(characterGroupId));

        CharacterGroupModel[] characterGroupModels = await m_applicationDictionaryService.GetCharacterGroupsAsync(characterGroupIdsWrapped, cancellationToken);

        return Ok(characterGroupModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterGroupModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterGroupModel[]>> GetCharacterGroupsByCaptionAsync([Required][FromQuery] string[] characterGroupCaptions, CancellationToken cancellationToken)
    {
        IEnumerable<CharacterGroupCaption> characterGroupCaptionsWrapped =
            characterGroupCaptions.Select(characterGroupCaption => new CharacterGroupCaption(characterGroupCaption));

        CharacterGroupModel[] characterGroupModels = await m_applicationDictionaryService.GetCharacterGroupsAsync(characterGroupCaptionsWrapped, cancellationToken);

        return Ok(characterGroupModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterGroupModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterGroupModel[]>> GetAllCharacterGroupsAsync(CancellationToken cancellationToken)
    {
        CharacterGroupModel[] characterGroupModels = await m_applicationDictionaryService.GetAllCharacterGroupsAsync(cancellationToken);

        return Ok(characterGroupModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CharacterGroupModel>> AddCharacterGroupAsync([Required][FromBody] CharacterGroupModel characterGroupModel, CancellationToken cancellationToken)
    {
        try
        {
            CharacterGroupModel addedCharacterGroupModel = await m_applicationDictionaryService.AddCharacterGroupAsync(characterGroupModel, cancellationToken);

            return Ok(addedCharacterGroupModel);
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to add the character group.");

            return Conflict();
        }
    }

    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateCharacterGroupAsync([Required][FromBody] CharacterGroupModel characterGroupModel, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await m_applicationDictionaryService.UpdateCharacterGroupAsync(characterGroupModel, cancellationToken);

            return updated ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to update the character group.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveCharacterGroupAsync([Required][FromQuery] int characterGroupId, CancellationToken cancellationToken)
    {
        try
        {
            CharacterGroupId characterGroupIdWrapped = new(characterGroupId);

            bool removed = await m_applicationDictionaryService.RemoveCharacterGroupAsync(characterGroupIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the character group.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveCharacterGroupByCaptionAsync([Required][FromQuery] string characterGroupCaption, CancellationToken cancellationToken)
    {
        try
        {
            CharacterGroupCaption characterGroupCaptionWrapped = new(characterGroupCaption);

            bool removed = await m_applicationDictionaryService.RemoveCharacterGroupAsync(characterGroupCaptionWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the character group.");

            return Conflict();
        }
    }

    #endregion

    #region IWordService Service Members

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WordModel>> GetWordAsync([Required][FromQuery] int wordId, CancellationToken cancellationToken)
    {
        WordId wordIdWrapped = new(wordId);

        WordModel? wordModel = await m_applicationDictionaryService.GetWordAsync(wordIdWrapped, cancellationToken);

        ActionResult<WordModel> actionResult = wordModel is not null
            ? (ActionResult<WordModel>)Ok(wordModel)
            : (ActionResult<WordModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordModel[]>> GetWordsAsync([Required][FromQuery] int[] wordIds, CancellationToken cancellationToken)
    {
        IEnumerable<WordId> wordIdsWrapped =
            wordIds.Select(wordId => new WordId(wordId));

        WordModel[] wordModels = await m_applicationDictionaryService.GetWordsAsync(wordIdsWrapped, cancellationToken);

        return Ok(wordModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordModel[]>> GetAllWordsAsync(CancellationToken cancellationToken)
    {
        WordModel[] wordModels = await m_applicationDictionaryService.GetAllWordsAsync(cancellationToken);

        return Ok(wordModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WordModel>> AddWordAsync([Required][FromBody] WordModel wordModel, CancellationToken cancellationToken)
    {
        try
        {
            WordModel addedWordModel = await m_applicationDictionaryService.AddWordAsync(wordModel, cancellationToken);

            return Ok(addedWordModel);
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to add the word.");

            return Conflict();
        }
    }

    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateWordAsync([Required][FromBody] WordModel wordModel, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await m_applicationDictionaryService.UpdateWordAsync(wordModel, cancellationToken);

            return updated ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to update the word.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveWordAsync([Required][FromQuery] int wordId, CancellationToken cancellationToken)
    {
        try
        {
            WordId wordIdWrapped = new(wordId);

            bool removed = await m_applicationDictionaryService.RemoveWordAsync(wordIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the word.");

            return Conflict();
        }
    }

    #endregion

    #region IWordGroupService Service Members

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WordGroupModel>> GetWordGroupAsync([Required][FromQuery] int wordGroupId, CancellationToken cancellationToken)
    {
        WordGroupId wordGroupIdWrapped = new(wordGroupId);

        WordGroupModel? wordGroupModel = await m_applicationDictionaryService.GetWordGroupAsync(wordGroupIdWrapped, cancellationToken);

        ActionResult<WordGroupModel> actionResult = wordGroupModel is not null
            ? (ActionResult<WordGroupModel>)Ok(wordGroupModel)
            : (ActionResult<WordGroupModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WordGroupModel>> GetWordGroupByCaptionAsync([Required][FromQuery] string wordGroupCaption, CancellationToken cancellationToken)
    {
        WordGroupCaption wordGroupCaptionWrapped = new(wordGroupCaption);

        WordGroupModel? wordGroupModel = await m_applicationDictionaryService.GetWordGroupAsync(wordGroupCaptionWrapped, cancellationToken);

        ActionResult<WordGroupModel> actionResult = wordGroupModel is not null
            ? (ActionResult<WordGroupModel>)Ok(wordGroupModel)
            : (ActionResult<WordGroupModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordGroupModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordGroupModel[]>> GetWordGroupsAsync([Required][FromQuery] int[] wordGroupIds, CancellationToken cancellationToken)
    {
        IEnumerable<WordGroupId> wordGroupIdsWrapped =
            wordGroupIds.Select(wordGroupId => new WordGroupId(wordGroupId));

        WordGroupModel[] wordGroupModels = await m_applicationDictionaryService.GetWordGroupsAsync(wordGroupIdsWrapped, cancellationToken);

        return Ok(wordGroupModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordGroupModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordGroupModel[]>> GetWordGroupsByCaptionAsync([Required][FromQuery] string[] wordGroupCaptions, CancellationToken cancellationToken)
    {
        IEnumerable<WordGroupCaption> wordGroupCaptionsWrapped =
            wordGroupCaptions.Select(wordGroupCaption => new WordGroupCaption(wordGroupCaption));

        WordGroupModel[] wordGroupModels = await m_applicationDictionaryService.GetWordGroupsAsync(wordGroupCaptionsWrapped, cancellationToken);

        return Ok(wordGroupModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordGroupModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordGroupModel[]>> GetAllWordGroupsAsync(CancellationToken cancellationToken)
    {
        WordGroupModel[] wordGroupModels = await m_applicationDictionaryService.GetAllWordGroupsAsync(cancellationToken);

        return Ok(wordGroupModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WordGroupModel>> AddWordGroupAsync([Required][FromBody] WordGroupModel wordGroupModel, CancellationToken cancellationToken)
    {
        try
        {
            WordGroupModel addedWordGroupModel = await m_applicationDictionaryService.AddWordGroupAsync(wordGroupModel, cancellationToken);

            return Ok(addedWordGroupModel);
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to add the word group.");

            return Conflict();
        }
    }

    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateWordGroupAsync([Required][FromBody] WordGroupModel wordGroupModel, CancellationToken cancellationToken)
    {
        try
        {
            bool updated = await m_applicationDictionaryService.UpdateWordGroupAsync(wordGroupModel, cancellationToken);

            return updated ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to update the word group.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveWordGroupAsync([Required][FromQuery] int wordGroupId, CancellationToken cancellationToken)
    {
        try
        {
            WordGroupId wordGroupIdWrapped = new(wordGroupId);

            bool removed = await m_applicationDictionaryService.RemoveWordGroupAsync(wordGroupIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the word group.");

            return Conflict();
        }
    }

    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveWordGroupByCaptionAsync([Required][FromQuery] string wordGroupCaption, CancellationToken cancellationToken)
    {
        try
        {
            WordGroupCaption wordGroupCaptionWrapped = new(wordGroupCaption);

            bool removed = await m_applicationDictionaryService.RemoveWordGroupAsync(wordGroupCaptionWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the word group.");

            return Conflict();
        }
    }

    #endregion
}
