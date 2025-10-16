using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using JapaneseLanguageTools.Contracts.Identifiers;
using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Requests;
using JapaneseLanguageTools.Contracts.Models.Responses;
using JapaneseLanguageTools.Contracts.Services.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JapaneseLanguageTools.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApplicationDictionaryExerciseController : ControllerBase
{
    private readonly ICharacterExerciseService m_characterExerciseService;
    private readonly IWordExerciseService m_wordExerciseService;
    private readonly ILogger<ApplicationDictionaryExerciseController> m_logger;

    public ApplicationDictionaryExerciseController(ICharacterExerciseService characterExerciseService, IWordExerciseService wordExerciseService, ILogger<ApplicationDictionaryExerciseController> logger)
    {
        m_characterExerciseService = characterExerciseService;
        m_wordExerciseService = wordExerciseService;
        m_logger = logger;
    }

    #region ICharacterExerciseService Service Members - CRUD Methods

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterExerciseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterExerciseModel>> GetCharacterExerciseAsync([Required][FromQuery] int characterExerciseId, CancellationToken cancellationToken = default)
    {
        CharacterExerciseId characterExerciseIdWrapped = new(characterExerciseId);

        CharacterExerciseModel? characterExerciseModel = await m_characterExerciseService.GetCharacterExerciseAsync(characterExerciseIdWrapped, cancellationToken);

        ActionResult<CharacterExerciseModel> actionResult = characterExerciseModel is not null
            ? (ActionResult<CharacterExerciseModel>)Ok(characterExerciseModel)
            : (ActionResult<CharacterExerciseModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterExerciseModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterExerciseModel[]>> GetCharacterExercisesAsync(IEnumerable<int> characterExerciseIds, CancellationToken cancellationToken = default)
    {
        IEnumerable<CharacterExerciseId> characterExerciseIdsWrapped =
            characterExerciseIds.Select(characterExerciseId => new CharacterExerciseId(characterExerciseId));

        CharacterExerciseModel[] characterExerciseModels = await m_characterExerciseService.GetCharacterExercisesAsync(characterExerciseIdsWrapped, cancellationToken);

        return Ok(characterExerciseModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterExerciseModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CharacterExerciseModel[]>> GetAllCharacterExercisesAsync(CancellationToken cancellationToken = default)
    {
        CharacterExerciseModel[] characterExerciseModels = await m_characterExerciseService.GetAllCharacterExercisesAsync(cancellationToken);

        return Ok(characterExerciseModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<CharacterExerciseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CharacterExerciseModel>> AddCharacterExerciseAsync(CharacterExerciseModel characterExerciseModel, CancellationToken cancellationToken = default)
    {
        try
        {
            CharacterExerciseModel addedCharacterExerciseModel = await m_characterExerciseService.AddCharacterExerciseAsync(characterExerciseModel, cancellationToken);

            return Ok(addedCharacterExerciseModel);
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
    public async Task<ActionResult<bool>> UpdateCharacterExerciseAsync(CharacterExerciseModel characterExerciseModel, CancellationToken cancellationToken = default)
    {
        try
        {
            bool updated = await m_characterExerciseService.UpdateCharacterExerciseAsync(characterExerciseModel, cancellationToken);

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
    public async Task<ActionResult<bool>> RemoveCharacterExerciseAsync(int characterExerciseId, CancellationToken cancellationToken = default)
    {
        try
        {
            CharacterExerciseId characterExerciseIdWrapped = new(characterExerciseId);

            bool removed = await m_characterExerciseService.RemoveCharacterExerciseAsync(characterExerciseIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the character.");

            return Conflict();
        }
    }

    #endregion

    #region ICharacterExerciseService Service Members - Request & Response Methods

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GetCharacterExerciseBatchResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCharacterExerciseBatchResponseModel>> GetCharacterExerciseBatchAsync([Required][FromQuery] Guid characterExerciseBatchId, CancellationToken cancellationToken)
    {
        GetCharacterExerciseBatchRequestModel getCharacterExerciseBatchRequestModel = new()
        {
            CharacterExerciseBatchId = characterExerciseBatchId,
        };

        GetCharacterExerciseBatchResponseModel? getCharacterExerciseBatchResponseModel =
            await m_characterExerciseService.GetCharacterExerciseBatchAsync(getCharacterExerciseBatchRequestModel, cancellationToken);

        ActionResult<GetCharacterExerciseBatchResponseModel> actionResult = getCharacterExerciseBatchResponseModel is not null
            ? (ActionResult<GetCharacterExerciseBatchResponseModel>)Ok(getCharacterExerciseBatchResponseModel)
            : (ActionResult<GetCharacterExerciseBatchResponseModel>)NotFound();

        return actionResult;
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GenerateCharacterExerciseBatchResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GenerateCharacterExerciseBatchResponseModel>> GenerateCharacterExerciseBatchAsync([Required][FromBody] GenerateCharacterExerciseBatchRequestModel generateCharacterExerciseBatchRequestModel, CancellationToken cancellationToken)
    {
        GenerateCharacterExerciseBatchResponseModel generateCharacterExerciseBatchResponseModel =
            await m_characterExerciseService.GenerateCharacterExerciseBatchAsync(generateCharacterExerciseBatchRequestModel, cancellationToken);

        return Ok(generateCharacterExerciseBatchResponseModel);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<SubmitCompletedCharacterExerciseDetailsResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SubmitCompletedCharacterExerciseDetailsResponseModel>> SubmitCompletedCharacterExerciseDetailsAsync([Required][FromBody] SubmitCompletedCharacterExerciseDetailsRequestModel submitCompletedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken)
    {
        SubmitCompletedCharacterExerciseDetailsResponseModel submitCompletedCharacterExerciseDetailsResponseModel =
            await m_characterExerciseService.SubmitCompletedCharacterExerciseDetailsAsync(submitCompletedCharacterExerciseDetailsRequestModel, cancellationToken);

        return Ok(submitCompletedCharacterExerciseDetailsResponseModel);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<SubmitFailedCharacterExerciseDetailsResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SubmitFailedCharacterExerciseDetailsResponseModel>> SubmitFailedCharacterExerciseDetailsAsync([Required][FromBody] SubmitFailedCharacterExerciseDetailsRequestModel submitFailedCharacterExerciseDetailsRequestModel, CancellationToken cancellationToken)
    {
        SubmitFailedCharacterExerciseDetailsResponseModel submitFailedCharacterExerciseDetailsResponseModel =
            await m_characterExerciseService.SubmitFailedCharacterExerciseDetailsAsync(submitFailedCharacterExerciseDetailsRequestModel, cancellationToken);

        return Ok(submitFailedCharacterExerciseDetailsResponseModel);
    }

    #endregion

    #region IWordExerciseService Service Members - CRUD Methods

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordExerciseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WordExerciseModel>> GetWordExerciseAsync([Required][FromQuery] int wordExerciseId, CancellationToken cancellationToken = default)
    {
        WordExerciseId wordExerciseIdWrapped = new(wordExerciseId);

        WordExerciseModel? wordExerciseModel = await m_wordExerciseService.GetWordExerciseAsync(wordExerciseIdWrapped, cancellationToken);

        ActionResult<WordExerciseModel> actionResult = wordExerciseModel is not null
            ? (ActionResult<WordExerciseModel>)Ok(wordExerciseModel)
            : (ActionResult<WordExerciseModel>)NotFound();

        return actionResult;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordExerciseModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordExerciseModel[]>> GetWordExercisesAsync(IEnumerable<int> wordExerciseIds, CancellationToken cancellationToken = default)
    {
        IEnumerable<WordExerciseId> wordExerciseIdsWrapped =
            wordExerciseIds.Select(wordExerciseId => new WordExerciseId(wordExerciseId));

        WordExerciseModel[] wordExerciseModels = await m_wordExerciseService.GetWordExercisesAsync(wordExerciseIdsWrapped, cancellationToken);

        return Ok(wordExerciseModels);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordExerciseModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WordExerciseModel[]>> GetAllWordExercisesAsync(CancellationToken cancellationToken = default)
    {
        WordExerciseModel[] wordExerciseModels = await m_wordExerciseService.GetAllWordExercisesAsync(cancellationToken);

        return Ok(wordExerciseModels);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<WordExerciseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WordExerciseModel>> AddWordExerciseAsync(WordExerciseModel wordExerciseModel, CancellationToken cancellationToken = default)
    {
        try
        {
            WordExerciseModel addedWordExerciseModel = await m_wordExerciseService.AddWordExerciseAsync(wordExerciseModel, cancellationToken);

            return Ok(addedWordExerciseModel);
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
    public async Task<ActionResult<bool>> UpdateWordExerciseAsync(WordExerciseModel wordExerciseModel, CancellationToken cancellationToken = default)
    {
        try
        {
            bool updated = await m_wordExerciseService.UpdateWordExerciseAsync(wordExerciseModel, cancellationToken);

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
    public async Task<ActionResult<bool>> RemoveWordExerciseAsync(int wordExerciseId, CancellationToken cancellationToken = default)
    {
        try
        {
            WordExerciseId wordExerciseIdWrapped = new(wordExerciseId);

            bool removed = await m_wordExerciseService.RemoveWordExerciseAsync(wordExerciseIdWrapped, cancellationToken);

            return removed ? Ok() : NotFound();
        }
        catch (DbUpdateException exception)
        {
            m_logger.LogWarning(exception, "An exception was thrown during the attempt to remove the word.");

            return Conflict();
        }
    }

    #endregion

    #region IWordExerciseService Service Members - Request & Response Methods

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GetWordExerciseBatchResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetWordExerciseBatchResponseModel>> GetWordExerciseBatchAsync([Required][FromQuery] Guid wordExerciseBatchId, CancellationToken cancellationToken)
    {
        GetWordExerciseBatchRequestModel getWordExerciseBatchRequestModel = new()
        {
            WordExerciseBatchId = wordExerciseBatchId,
        };

        GetWordExerciseBatchResponseModel? getWordExerciseBatchResponseModel =
            await m_wordExerciseService.GetWordExerciseBatchAsync(getWordExerciseBatchRequestModel, cancellationToken);

        ActionResult<GetWordExerciseBatchResponseModel> actionResult = getWordExerciseBatchResponseModel is not null
            ? (ActionResult<GetWordExerciseBatchResponseModel>)Ok(getWordExerciseBatchResponseModel)
            : (ActionResult<GetWordExerciseBatchResponseModel>)NotFound();

        return actionResult;
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GenerateWordExerciseBatchResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GenerateWordExerciseBatchResponseModel>> GenerateWordExerciseBatchAsync([Required][FromBody] GenerateWordExerciseBatchRequestModel generateWordExerciseBatchRequestModel, CancellationToken cancellationToken)
    {
        GenerateWordExerciseBatchResponseModel generateWordExerciseBatchResponseModel =
            await m_wordExerciseService.GenerateWordExerciseBatchAsync(generateWordExerciseBatchRequestModel, cancellationToken);

        return Ok(generateWordExerciseBatchResponseModel);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<SubmitCompletedWordExerciseDetailsResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SubmitCompletedWordExerciseDetailsResponseModel>> SubmitCompletedWordExerciseDetailsAsync([Required][FromBody] SubmitCompletedWordExerciseDetailsRequestModel submitCompletedWordExerciseDetailsRequestModel, CancellationToken cancellationToken)
    {
        SubmitCompletedWordExerciseDetailsResponseModel submitCompletedWordExerciseDetailsResponseModel =
            await m_wordExerciseService.SubmitCompletedWordExerciseDetailsAsync(submitCompletedWordExerciseDetailsRequestModel, cancellationToken);

        return Ok(submitCompletedWordExerciseDetailsResponseModel);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType<SubmitFailedWordExerciseDetailsResponseModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SubmitFailedWordExerciseDetailsResponseModel>> SubmitFailedWordExerciseDetailsAsync([Required][FromBody] SubmitFailedWordExerciseDetailsRequestModel submitFailedWordExerciseDetailsRequestModel, CancellationToken cancellationToken)
    {
        SubmitFailedWordExerciseDetailsResponseModel submitFailedWordExerciseDetailsResponseModel =
            await m_wordExerciseService.SubmitFailedWordExerciseDetailsAsync(submitFailedWordExerciseDetailsRequestModel, cancellationToken);

        return Ok(submitFailedWordExerciseDetailsResponseModel);
    }

    #endregion
}
