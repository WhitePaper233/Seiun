using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Resources;
using Seiun.Models.Responses;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;


namespace Seiun.Controllers;

/// <summary>
/// 题目控制器
/// </summary>
/// <param name="repository">日志</param>
[ApiController]
[Route("/api/question")]
public class QuestionController(IRepositoryService repository) : ControllerBase
{
    /// <summary>
    /// 获取题目列表
    /// </summary>
    /// <returns>题目列表</returns>
    [HttpGet("list", Name = "QuestionList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> QuestionList([FromQuery] string questionType)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, QuestionListResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        List<Guid>? questionList;
        switch (questionType)
        {
            case "":
            case "all":
                questionList = (await repository.UserQuestionRepository.GetByUserId(userId.Value))
                    .Select(u => u.QuestionId).ToList();
                break;
            case "fill-in-blank":
                questionList =
                    (await repository.UserQuestionRepository.GetByUserIdAndQuestionType(userId.Value,
                        QuestionType.FillInBlank)).Select(u => u.QuestionId).ToList();
                break;
            case "cloze-test":
                questionList =
                    (await repository.UserQuestionRepository.GetByUserIdAndQuestionType(userId.Value,
                        QuestionType.ClozeTest)).Select(u => u.QuestionId).ToList();
                break;
            default:
                return BadRequest(QuestionListResp.Fail(
                    StatusCodes.Status400BadRequest,
                    ErrorMessages.Controller.Any.InvalidReqType
                ));
        }

        if (questionList.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, QuestionListResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionNotFound
            ));

        return Ok(QuestionListResp.Success(questionList));
    }

    /// <summary>
    /// 获取选词填空题目
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns>题目</returns>
    [HttpGet("fill-blank", Name = "FillBlank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FillInBlank([FromQuery] Guid questionId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, FillInBlankResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var question = await repository.FillInBlankRepository.GetByIdAsync(questionId);
        if (question == null)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionNotFound
            ));

        var questionWord = await repository.FillInBlankWordRepository.GetByQuestionIdAsync(questionId);
        if (questionWord == null || questionWord.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionWordNotFound
            ));

        var questionAnswer = await repository.FillInBlankAnswerRepository.GetByQuestionIdAsync(questionId);
        if (questionAnswer == null || questionAnswer.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionAnswerNotFound
            ));

        var qes = new FillInBlankInfo
        {
            Words = questionWord.Select(a => a.Word).ToList(),
            Content = question.Content,
            Transition = question.Transition,
            Answers = questionAnswer.Select(a =>
                new FillInBlankAnswerInfo
                {
                    Key = a.Key,
                    Answer = a.Answer,
                    Analysis = a.Analysis
                }).ToList()
        };

        return Ok(FillInBlankResp.Success(qes));
    }

    /// <summary>
    /// 获取完形填空题目
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns>题目</returns>
    [HttpGet("cloze-test", Name = "ClozeTest")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClozeTest([FromQuery] Guid questionId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ClozeTestResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var question = await repository.ClozeTestRepository.GetByIdAsync(questionId);
        if (question == null)
            return StatusCode(StatusCodes.Status404NotFound, ClozeTestResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionNotFound
            ));

        var questionSelection = await repository.ClozeTestSelectionRepository.GetByQuestionIdAsync(questionId);
        if (questionSelection == null || questionSelection.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, ClozeTestResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionWordNotFound
            ));

        var questionAnswer = await repository.ClozeTestAnswerRepository.GetByQuestionIdAsync(questionId);
        if (questionAnswer == null || questionAnswer.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, ClozeTestResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Question.QuestionAnswerNotFound
            ));

        var qes = new ClozeTestInfo
        {
            Selections = questionSelection.Select(q =>
                new ClozeTestSelectionInfo
                {
                    Key = q.Key,
                    Words = q.Words
                }).ToList(),
            Content = question.Content,
            Answers = questionAnswer.Select(q =>
                new ClozeTestAnswerInfo
                {
                    Key = q.Key,
                    Answer = q.Answer,
                    Analysis = q.Analysis
                }).ToList()
        };

        return Ok(ClozeTestResp.Success(qes));
    }
}