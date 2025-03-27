namespace Seiun.Resources;

public static class SuccessMessages
{
    public static class Controller
    {
        public static class User
        {
            public const string RegisterSuccess = "controller.user.register.register_success";
            public const string LoginSuccess = "controller.user.login.login_success";
            public const string TokenRefreshSuccess = "controller.user.refresh_token.token_refresh_success";
            public const string ProfileUpdateSuccess = "controller.user.profile.update_success";
            public const string AvatarUpdateSuccess = "controller.user.profile.avatar_update_success";
            public const string GetProfileSuccess = "controller.user.profile.get_success";
        }

        public static class Admin
        {
            public const string DeleteSuccess = "controller.admin.delete.delete_success";
            public const string UpdateSuccess = "controller.admin.update.update_success";
            public const string GetUserListSuccess = "controller.admin.get.get_userlist_success";
            public const string GetRoleListSuccess = "controller.admin.get.get_rolelist_success";
            public const string ChangeRoleSuccess = "controller.admin.change.change_role_success";
        }

        public static class Article
        {
            public const string CreateSuccess = "controller.article.create.create_success";
            public const string DeleteSuccess = "controller.article.delete.delete_success";
            public const string PinSuccess = "controller.article.pin.pin_success";
            public const string PinCancelSuccess = "controller.article.pin.pin_cancel_success";
            public const string GetArticleListSuccess = "controller.article.GetArticleList.get_articlelist_success";

            public const string GetArticleDetailSuccess =
                "controller.article.GetArticleDetail.get_articledetail_success";

            public const string LikeSuccess = "controller.article.like.like_success";

            public const string GetArticleImgNameSuccess =
                "controller.article.articleimgs.get_articleimgname_success";
        }

        public static class PublicAnnouncement
        {
            public const string PublishSuccess = "controller.publicannouncement.publish.publish_success";
            public const string DeleteSuccess = "controller.publicannouncement.delete.delete_success";

            public const string GetPublicAnnouncementsSuccess =
                "controller.publicannouncement.get.get_publicannouncements_success";
        }

        public static class Comment
        {
            public const string CreateSuccess = "controller.comment.create.create_success";
            public const string DetailSuccess = "controller.comment.detail.detail_success";
            public const string DeleteSuccess = "controller.comment.delete.delete_success";
            public const string GetListSuccess = "controller.comment.get.list_success";
            public const string GetLikeSuccess = "controller.comment.get.like_success";
            public const string CancelLikeSuccess = "controller.comment.cancel.cancel_like_success";
            public const string GetDislikeSuccess = "controller.comment.get.dis_like_success";
            public const string CancelDislikeSuccess = "controller.comment.cancel.cancel_dislike_success";
        }

        public static class Reply
        {
            public const string CreateSuccess = "controller.reply.create.create_success";
            public const string DeleteSuccess = "controller.reply.delete.delete_success";
            public const string GetListSuccess = "controller.reply.get.list_success";
            public const string DetailSuccess = "controller.reply.detail.detail_success";
        }

        public static class WordBook
        {
            public const string GetWordBooksSuccess = "controller.word_book.get_word_books_success";
            public const string SelectWordBookSuccess = "controller.word_book.select_word_book_success";
        }

        public static class UserPlan
        {
            public const string GetCurrentPlanSuccess = "controller.user_plan.get_current_plan_success";
            public const string UpdatePlanSuccess = "controller.user_plan.update_plan.success";
        }

        public static class StudySession
        {
            public const string GetSessionDetailSuccess = "controller.studysession.get.detail_success";
            public const string GetNextWordSuccess = "controller.studysession.get.next_word_success";
            public const string ContinueSessionSuccess = "controller.studysession.continue_session_success";
        }

        public static class Word
        {
            public const string FinishedWordCreatSuccess = "controller.word.finishedword.create.create_success";
            public const string ErrorWordRecordCreatSuccess = "controller.word.errorword.record.record_success";
            public const string GetWordSuccess = "controller.word.get.word_success";
        }

        public static class WordSession
        {
            public const string WordSessionOver = "controller.session.word_session_over";
        }

        public static class Challenge
        {
            public const string GetFillInBlankSuccess = "controller.question.get.fill_in_blank_success";
            public const string GetClozeTestSuccess = "controller.question.get.cloze_test.success";
            public const string GetChallengeListSuccess = "controller.question.get.list_success";
        }

        public static class CheckIn
        {
            public const string GetCheckInStatusSuccess = "controller.checkin.get_check_in_status_success";
            public const string GetCheckInDaysSuccess = "controller.checkin.get_check_in_days_success";
        }
    }
}