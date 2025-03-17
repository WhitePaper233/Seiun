namespace Seiun.Resources;

public static class SuccessMessages
{
    public static class Controller
    {
        public static class User
        {
            public const string RegisterSuccess = "controller.user.register.register_success";
            public const string LoginSuccess = "controller.user.login.login_success";
            public const string ProfileUpdateSuccess = "controller.user.profile.update_success";
            public const string AvatarUpdateSuccess = "controller.user.profile.avatar_update_success";
            public const string GetProfileSuccess = "controller.user.profile.get_success";
            public const string CheckInToday = "controller.user.checkin.checkin_today";
            public const string NoCheckInToday = "controller.user.checkin.no_checkin_today";
            public const string NoCheckInHistory = "controller.user.checkin.no_checkin_history";
            public const string GetConsecutiveCheckInDays = "controller.user.checkin.get_consecutive_checkin_days";
        }
        public static class Admin
        {
            public const string DeleteSuccess = "controller.admin.delete.delete_success";
            public const string UpdateSuccess = "controller.admin.update.update_success";
            public const string GetUserListSuccess = "controller.admin.get.get_userlist_success";
        }
        public static class Article
        {
            public const string CreateSuccess = "controller.article.create.create_success";
            public const string DeleteSuccess = "controller.article.delete.delete_success";
            public const string PinSuccess = "controller.article.pin.pin_success";
            public const string PinCancelSuccess = "controller.article.pin.pin_cancel_success";
            public const string GetArticleListSuccess = "controller.article.GetArticleList.get_articlelist_success";
            public const string GetArticleDetailSuccess = "controller.article.GetArticleDetail.get_articledetail_success";
            public const string LikeSuccess = "controller.article.like.like_success";
            public const string GetArticleImgNameListSuccess = "controller.article.articleimgs.get_articleimgnamelist_success";
        }
    
        public static class PublicAnnouncement
        {
            public const string PublishSuccess = "controller.publicannouncement.publish.publish_success";
            public const string DeleteSuccess = "controller.publicannouncement.delete.delete_success";
            public const string GetPublicAnnouncementsSuccess = "controller.publicannouncement.get.get_publicannouncements_success";
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

        public static class UserTag
        {
            public const string CreateSuccess = "controller.usertag.create.create_success";
            public const string DeleteSuccess = "controller.usertag.delete.delete_success";
            public const string GetDetailSuccess = "controller.usertag.get.detail_success";
        }

        public static class StudySession
        {
            public const string GetSessionDetailSuccess = "controller.studysession.get.detail_success";
            public const string GetNextWordSuccess = "controller.studysession.get.next_word_success";
        }

        public static class Word
        {
            public const string GetReviewingWordSuccess = "controller.word.get.reviewing_word_success";
            public const string FinishedWordCreatSuccess = "controller.word.finishedword.create.create_success";
            public const string ErrorWordRecordCreatSuccess = "controller.word.errorword.record.record_success";
        }

        public static class Session
        {
            public const string WordSessionOver = "controller.session.word_session_over";
            public const string SessionOverSuccess = "controller.session.session_over_success";
        }
    }
}