using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Resources;
using Seiun.Models.Responses;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;
using Seiun.Entities;


namespace Seiun.Controllers;

/// <summary>
/// 题目控制器
/// </summary>
/// <param name="logger">日志</param>
/// <param name="repository">日志</param>
/// <param name="aiRequest">AI请求服务</param>
[ApiController, Route("/api/question")]
public class QuestionController(ILogger<QuestionController> logger, IRepositoryService repository, IAIRequestService aiRequest) : ControllerBase
{
    /// <summary>
    /// 获取完型填空
    /// </summary>
    /// <returns>完型填空题目</returns>
    [HttpGet("fill-blank", Name = "FillBlank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FillBlank()
    {
        var userId = User.GetUserId();
        if(userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, FillInBlankResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }
        
        var finishedWordEntities = await repository.FinishedWordRepository.GetWordsToQuestionAsync(userId.Value);
        if (finishedWordEntities == null)
        {
            return NotFound(FillInBlankResp.Fail(
            StatusCodes.Status404NotFound,
            ErrorMessages.Controller.Question.NoWordsToQuestion
            ));
        }

        var wordsToQuestion =
            (await repository.WordRepository.GetByGuidsAsync(finishedWordEntities.Select(x => x.WordId).ToList()))
            .ToList();
        if (wordsToQuestion.Count == 0)
        {
            return NotFound(FillInBlankResp.Fail(
            StatusCodes.Status404NotFound,
            ErrorMessages.Controller.Word.WordNotFound
            ));
        }

        var question = await aiRequest.GetAiFillInBlankAsync([.. wordsToQuestion.Select(x => x.WordText)]);
        if (question != null)
        {
            var fillInBlankEntity = new FillInBlankEntity
            {
                Content = question.Content,
                Transition = question.Transition
            };
            repository.FillInBlankRepository.Create(fillInBlankEntity);

            var fillInBlankAnswerEntity = question.Answers.Select(a =>
                new FillInBlankAnswerEntity
                {
                    QuestionId = fillInBlankEntity.Id,
                    Key = a.Key,
                    Answer = a.Answer,
                    Analysis = a.Analysis
                }).ToList();
            repository.FillInBlankAnswerRepository.BulkAdd(fillInBlankAnswerEntity);
            
            var fillInBlankWordEntity = question.Words.Select(w =>
                new FillInBlankWordEntity
                {
                    QuestionId = fillInBlankEntity.Id,
                    Word = w
                }).ToList();
            repository.FillInBlankWordRepository.BulkAdd(fillInBlankWordEntity);

            if (await repository.FillInBlankRepository.SaveAsync() &&
                await repository.FillInBlankRepository.SaveAsync() &&
                await repository.FillInBlankRepository.SaveAsync())
            {
                return Ok(FillInBlankResp.Success(question));
            }
        }

        logger.LogWarning("User {} get ai fill in blank failed", userId.Value);
        return StatusCode(StatusCodes.Status500InternalServerError, FillInBlankResp.Fail(
            StatusCodes.Status500InternalServerError, 
            ErrorMessages.Controller.Question.GetAiFillInBlankFailed
        ));
    }
    
    
}