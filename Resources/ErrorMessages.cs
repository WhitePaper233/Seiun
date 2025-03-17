using Seiun.Models.Parameters;

namespace Seiun.Resources;

public static class ErrorMessages
{
    public static class ValidationError
    {
        public const string PhoneRequired = "error.validation.phone_number_required";
        public const string PasswordRequired = "error.validation.password_required";
        public const string PublicAnnouncementTitleRequired = "error.validation.public_announcement_title_required";
        public const string PublicAnnouncementContentRequired = "error.validation.public_announcement_content_required";
        public const string ArticleRequired = "error.validation.article_required";

        public const string InvalidPhone = "error.validation.invalid_phone_number";
        public const string InvalidEmail = "error.validation.invalid_email";
        public const string InvalidPassword = "error.validation.invalid_password";
        public const string InvalidUserName = "error.validation.invalid_username";
        public const string InvalidLikeCount = "error.validation.invalid_like_count";
        public const string InvalidDisLikeCount = "error.validation.invalid_dislike_count";
        
        public const string OverPhoneNumberLength = "error.validation.over_phone_number_length";
        public const string OverNickNameLength = "error.validation.over_nickname_length";
        public const string OverUserNameLength = "error.validation.over_username_length";
        public const string OverDescriptionLength = "error.validation.over_description_length";
        public const string OverEmailLength = "error.validation.over_email_length";
        public const string OverContentLength = "error.validation.over_content_length";
        public const string OverTagNameLength = "error.validation.over_tag_length";
        
        public const string AtLeastOnePropertyRequired = "error.validation.at_least_one_property_is_required";
        public const string UserIdRequired = "error.validation.user_id_required";
        public const string UserTagTotalDaysRequired = "error.validation.tag_total_days_required";
        public const string UserTagDailyPlanRequired = "error.validation.tag_daily_plan_required";
        public const string UserUpdateRequired = "error.validation.user_update_required";

        public const string CommentIdRequired = "error.validation.comment_id_required";

        public const string ContentRequired = "error.validation.Content_required";

        public const string ArticleIdRequired = "erroe.validation.post_id_required";

        public const string WordIdRequired = "error.validation.word_id_required";

        public const string SessionIdRequired = "error.validation.session_id_required";
        public const string OverArticleLength = "error.validation.over_article_length";
        public const string OverCoverUrlLength = "error.validation.over_cover_url_length";
        public const string OverPublicAnnouncementLength = "error.validation.over_public_announcement_length";
    }

    public static class Controller
    {
        public static class Any
        {
            public const string ParamValidFailed = $"error.controller.any.param_valid_failed";
            public const string FileNotUploaded = "error.controller.any.file_not_uploaded";
            public const string FileTooLarge = "error.controller.any.file_too_large";
            public const string FileFormatNotSupported = "error.controller.any.file_format_not_supported";
            public const string ImageSizeTooLarge = "error.controller.any.image_size_too_large";
            public const string UnknownFileProcessingError = "error.controller.any.unknown_file_processing_error";
            public const string InvalidJwtToken = "error.controller.any.invalid_jwt_token";
            public const string FileFormatNotJson = "error.controller.any.file_format_not_json";
        }
        public static class User
        {
            public const string UserNotFound = "error.controller.user.not_found";
            public const string UserLoginFailed = "error.controller.user.login_failed";
            public const string ProfileUpdateFailed = "error.controller.user.profile.update_failed";
            public const string PhoneNumberDuplicated = "error.controller.user.register.phone_number_already_exists";
            public const string RegisterFailed = "error.controller.user.register.register_failed";
            public const string UserCheckInFailed = "error.controller.user.checkin.checkin_failed";
        }
        public static class Admin
        {
            public const string ProfileUpdateFailed = "error.controller.admin.profile.update_failed";
            public const string UserLoginFailed = "error.controller.admin.login_failed";
            public const string AdminNotFound = "error.controller.admin.not_found";
            public const string UserListFailed = "error.controller.admin.list.get_userlist_failed";
        }
        public static class Article
        {
            public const string PermissonDeniedError = "error.controller.article.permission_denied";
            public const string CreateFailed = "error.controller.article.create.create_failed";
            public const string ArticleNotFound = "error.controller.article.not_found";
            public const string DeleteFailed = "error.controller.article.delete.delete_failed";
            public const string PinFailed = "error.controller.article.pin.pin_failed";
            public const string ArticlePinned = "error.controller.article.pin.article_is_pinned";
            public const string PinCancelFailed = "error.controller.article.cancelpin.pin_cancel_failed";
            public const string ArticleNotPinned = "error.controller.article.cancelpin.article_not_pinned";
            public const string UserIdRequired = "error.controller.article.getarticlelist.user_id_required";
            public const string ArticleListNotFound = "error.controller.article.getarticlelist.article_list_not_found";
            public const string InvalidReqType = "error.controller.article.getarticlelist.invalid_reqtype";
            public const string GetArticleListFailed = "error.controller.article.getarticlelist.get_articlelist_failed";
            public const string LikeFailed = "error.controller.article.like.like_failed";
            public const string ArticleLiked = "error.controller.article.like.article_is_liked";
            public const string ArticleNotLiked = "error.controller.article.cancellike.article_not_liked";
            public const string ArticleImgUploadFailed = "error.controller.article.uploadarticleimage.upload_failed";
            public const string AiArticleNotFound = "error.controller.article.getaiarticle.aiarticle_not_found";
        }

        public static class PublicAnnouncement
        {
            public const string PublishFailed = "error.controller.publicannouncement.publish.publish_failed";
            public const string AnnouncementNotFound = "error.controller.publicannouncement.not_found";
            public const string NotAuthorized = "error.controller.publicannouncement.not_authorized";
            public const string DeleteFailed = "error.controller.publicannouncement.delete.delete_failed";
        }
        public static class Comment
        {
            public const string CreateFailed = "error.controller.comment.create_failed";
            public const string CommentNotFound = "error.controller.comment.not_found";
            public const string CommentDeleteFailed = "error.controller.comment.delete_failed";
            public const string AlreadyLiked = "error.controller.comment.already_liked"; 
            public const string GetLikeFailed = "error.controller.comment.like_failed"; 
            public const string AlreadyCancelLiked = "error.controller.comment.already_cancel_liked"; 
            public const string CancelLikeFailed = "error.controller.comment.cancel_like_failed"; 
            public const string GetDislikeFailed = "error.controller.comment.get_dislike_failed";
            public const string AlreadyDisliked = "error.controller.comment.already_dislike";
            public const string AlreadyCancelDisliked = "error.controller.comment.already_cancel_dislike";
            public const string CancelDislikeFailed = "error.controller.comment.cancel_dislike_failed";
        }
        public static class Reply
        {
            public const string CreateFailed = "error.controller.reply.create_failed";
            public const string ReplyIdRequired = "error.controller.reply.replyid_required";
            public const string DeleteFailed = "error.controller.reply.delete_failed";
            public const string ParentReplyNotFound = "error.controller.reply.parent_reply_not_found";
            public const string ReplyNotFound = "error.controller.reply.not_found";
        }
        public static class Word
        {
            public const string FinishedWordCreatFailed = "error.controller.word.finishedword.create_failed";
            public const string ErrorWordCreatFailed = "error.controller.word.errorword.create_failed";
            public const string WordNotFound = "error.controller.word.not_found";
            public const string LatestWordNotFound = "error.controller.word.latest_word_not_found";
        }

        public static class Tag
        {
            public const string TagNotFound = "error.controller.tag.not_found";
        }
        public static class UserTag
        {
            public const string CreateFailed = "error.controller.user_tag.create_failed";
            public const string DeleteFailed = "error.controller.user_tag.delete_failed";
            public const string UserTagNotFound = "error.controller.user_tag.not_found";
        }
        public static class WordSession
        {
            public const string StartFailed = "error.controller.session.start_failed";
            public const string NotFoundSession = "error.controller.session.not_found_session";
            public const string GetNextWordFailed = "error.controller.session.get_next_word_failed";
            public const string DeleteFailed = "error.controller.session.delete_failed";
            public const string CreateAiArticleFailed = "error.controller.session.create_ai_article_failed";
            public const string CreateAiCoverFailed = "error.controller.session.create_ai_cover_failed";
        }

        public static class Question
        {
            public const string NoWordsToQuestion = "error.controller.question.no_words_to_question";
            public const string GetAiFillInBlankFailed = "error.controller.question.get_ai_fill_in_blank_failed";
        }
    }
}