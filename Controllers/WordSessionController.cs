using Seiun.Services;
using Seiun.Utils;
using Seiun.Resources;
using Seiun.Utils.Enums;
using Seiun.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Seiun.Entities;

namespace Seiun.Controllers;

[ApiController,Route("api/session")]
public class WordSessionController(ILogger<WordSessionController> logger, IRepositoryService repository, ICurrentStudySessionService currentStudySession, IAIRequestService aiRequest)
	: ControllerBase
{
	/// <summary>
	/// 开始学习单词会话
	/// </summary>
	/// <returns>会话信息</returns>
	[HttpPost("start", Name = "StartStudy")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
	[ProducesResponseType(typeof(StartStudyResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Init()
	{
		var userId = User.GetUserId();
		if (userId == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.Any.InvalidJwtToken
			));
		}

		var selectedTag = await repository.UserTagRepository.GetStudyingTagByUserIdAsync(userId.Value);
		if(selectedTag==null)
		{
			return NotFound(StartStudyResp.Fail(
				StatusCodes.Status404NotFound,
				ErrorMessages.Controller.UserTag.UserTagNotFound
			));
		}
	    
		var session = new WordSessionEntity
		{
            UserId = userId.Value,
			WordSessionAt = DateTime.Now,
		};
		var wordQUeue = new Queue<WordEntity>();

		int reviewingWordCount = 0;
		var reviewingWords = await repository.ErrorWordRepository.GetErrorWordIdsByUserIdAsync(userId.Value);
		if(reviewingWords!=null&&reviewingWords.Count>0)
		{
			reviewingWordCount = reviewingWords.Count;
			foreach(var wordId in reviewingWords)
			{
				var word = await repository.WordRepository.GetByIdAsync(wordId);
				if(word!=null)
				{
					wordQUeue.Enqueue(word);
				}
			}
		}

		int studyingWordCount = 0;
	    var studyWords = await repository.WordRepository.GetWordsByTagAsync(selectedTag.Tag.Name, userId.Value, selectedTag.DailyPlan);
		if(studyWords!=null&&studyWords.Count>0)
		{
			studyingWordCount = studyWords.Count;
			foreach(var word in studyWords)
			{
				wordQUeue.Enqueue(word);
			}
		}

		repository.SessionRepository.Create(session);
		var NewSessionResult = currentStudySession.AddSession(session.Id, wordQUeue, logger);
		if(NewSessionResult && await repository.SessionRepository.SaveAsync())
		{
			return Ok(StartStudyResp.Success(session.Id, reviewingWordCount, studyingWordCount));
		}
		
		logger.LogWarning("Start study session failed");
		return StatusCode(StatusCodes.Status500InternalServerError, StartStudyResp.Fail(
			StatusCodes.Status500InternalServerError,
			ErrorMessages.Controller.WordSession.StartFailed
		));
	}

	/// <summary>
 	/// 获取下一个单词
	/// </summary>
	/// <param name="sessionId">会话ID</param>
	/// <returns>下一个单词信息</returns>
	[HttpGet("nextword", Name = "GetNextWord")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(GetNextWordResp), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetNextWord([FromQuery] Guid sessionId){
        var userId = User.GetUserId();
		if (userId == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.Any.InvalidJwtToken
			));
		}

		var user = await repository.UserRepository.GetByIdAsync(userId.Value);
		if(user==null)
		{
			return NotFound(GetNextWordResp.Fail(
				StatusCodes.Status404NotFound,
				ErrorMessages.Controller.User.UserNotFound
			));
		}

		var session = await repository.SessionRepository.GetByIdAsync(sessionId);
		if(session==null || session.UserId!=userId.Value)
		{
			return NotFound(GetNextWordResp.Fail(
				StatusCodes.Status404NotFound,
				ErrorMessages.Controller.WordSession.NotFoundSession
			));
		}

		try
		{
			var word = currentStudySession.GetNextWord(sessionId, logger);
			if(word != null)
			{
				return Ok(GetNextWordResp.Success(word));
			}
			
			var latestFinishedWordGroup = await repository.FinishedWordRepository.GetLatestFinishedWordIdAsync(userId.Value);
			if(latestFinishedWordGroup == null)
			{
				return NotFound(AiArticleDetailResp.Fail(
					StatusCodes.Status404NotFound,
					ErrorMessages.Controller.Word.LatestWordNotFound
				));
			}
			
			var latestFinishedWordEntities = latestFinishedWordGroup.ToList();
			var latestFinishedWords =
				(await repository.WordRepository.GetByGuidsAsync([.. latestFinishedWordEntities.Select(x => x.WordId)]))
				.ToList(); 
			if(latestFinishedWords.Count == 0)
			{
				return NotFound(AiArticleDetailResp.Fail(
					StatusCodes.Status404NotFound,
					ErrorMessages.Controller.Word.LatestWordNotFound
				));
			}

			var aiArticle = await aiRequest.GetAIArticleAsync([.. latestFinishedWords.Select(x => x.WordText)]);
			if (aiArticle == null)
			{
				logger.LogError("AI Article is null");
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
					StatusCodes.Status500InternalServerError,
					ErrorMessages.Controller.WordSession.CreateAiArticleFailed
				));
			}

			var aiCover = await aiRequest.GetAICoverAsync(aiArticle);
			if (string.IsNullOrEmpty(aiCover))
			{
				logger.LogError("AI Cover is null");
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
					StatusCodes.Status500InternalServerError,
					ErrorMessages.Controller.WordSession.CreateAiCoverFailed
				));
			}

			var aIArticleEntity = new AiArticleEntity
			{
				UserId = userId.Value,
				SessionId = latestFinishedWordGroup.Key,
				Article = aiArticle,
				CoverUrl = aiCover,
				CreatedAt = DateTime.UtcNow
			};
			repository.AIArticleRepository.Create(aIArticleEntity);
			if(!await repository.AIArticleRepository.SaveAsync())
			{
				logger.LogWarning("User {} failed to Create AI article entity.", userId.Value);
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
					StatusCodes.Status500InternalServerError,
					ErrorMessages.Controller.WordSession.CreateAiArticleFailed
				));				
			}
			
			// 下一个单词为空，表示会话已经结束
			var userCheckInEntity = new UserCheckInEntity
			{
				UserId = userId.Value,
				CheckInDate = DateTime.Now,
				User = user
			};
			// 打卡
			if (await repository.UserCheckInRepository.CheckInTodayAsync(userId.Value))
			{
				repository.UserCheckInRepository.Create(userCheckInEntity);
			}
			else
			{
				repository.UserCheckInRepository.Update(userCheckInEntity);
			}

			// 删除会话
			currentStudySession.RemoveSession(session.Id, logger);
			// 删除会话记录表
			repository.SessionRepository.Delete(session);

			if(!await repository.UserCheckInRepository.SaveAsync())
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
					StatusCodes.Status500InternalServerError,
					ErrorMessages.Controller.User.UserCheckInFailed
				));
			}
			if(!await repository.SessionRepository.SaveAsync())
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
					StatusCodes.Status500InternalServerError,
					ErrorMessages.Controller.WordSession.DeleteFailed
				));
			}
			
			// 返回会话结束信息
			return Ok(SuccessMessages.Controller.WordSession.WordSessionOver);
		}
		catch (Exception e)
		{
			logger.LogWarning(e,"User {} get next word failed", userId);
			return StatusCode(StatusCodes.Status500InternalServerError, GetNextWordResp.Fail(
				StatusCodes.Status500InternalServerError,
				ErrorMessages.Controller.WordSession.GetNextWordFailed
			));
		}
	}	

	/// <summary>
	/// 提交单词结果
	/// </summary>
	/// <param name="wordResultDto"></param>
	/// <returns>操作结果</returns>
	[HttpPost("correct", Name = "Correct")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Correct([FromBody] WordResultDto wordResultDto){
        var userId = User.GetUserId();
		if (userId == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.Any.InvalidJwtToken
			));
		}

		var session = await repository.SessionRepository.GetByIdAsync(wordResultDto.SessionId);
		if(session==null || session.UserId!=userId.Value)
		{
			if(session==null || session.UserId!=userId.Value)
		{
			return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.WordSession.NotFoundSession
            ));
		}
		}

		var finishedRecord = new FinishedWordRecordEntity
			{
				UserId = userId.Value,
				SessionId = wordResultDto.SessionId,
				WordId = wordResultDto.WordId,
				FinishedAt = DateTime.UtcNow,
			};
		currentStudySession.DeleteCorrectWord(session.Id, logger);
		repository.FinishedWordRepository.Create(finishedRecord);
		if(await repository.FinishedWordRepository.SaveAsync())
		{
			return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Word.FinishedWordCreatSuccess));
		}
		
		logger.LogError("User {} finished word {} failed", userId, wordResultDto.WordId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Word.FinishedWordCreatFailed
        ));
	}

	/// <summary>
	/// 提交单词错误
	/// </summary>
	/// <param name="wordResultDto"></param>
	/// <returns>操作结果</returns>
	[HttpPost("error", Name = "Error")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Error([FromBody] WordResultDto wordResultDto){
        var userId = User.GetUserId();
		if (userId == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.Any.InvalidJwtToken
			));
        }
		var session = await repository.SessionRepository.GetByIdAsync(wordResultDto.SessionId);
		if(session==null || session.UserId!=userId.Value)
		{
			return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.WordSession.NotFoundSession
            ));
		}

		var errorRecord = new ErrorWordRecordEntity
			{
				UserId = userId.Value,
				SessionId = wordResultDto.SessionId,
				WordId = wordResultDto.WordId,
			};
		repository.ErrorWordRepository.Create(errorRecord);
		currentStudySession.InsertErrorWord(session.Id, logger);
		if(await repository.ErrorWordRepository.SaveAsync())
		{
			return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Word.ErrorWordRecordCreatSuccess));
		}

		logger.LogError("User {} error word {} failed", userId, wordResultDto.WordId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Word.ErrorWordCreatFailed
        ));
	}
}