using System;
using System.Linq;
using System.Web.Services;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using BL = GS.Common.BL;

namespace GradSovetPages.Layouts.GradSovetPages.Pages
{
    public partial class MeetingMvk : LayoutsPageBase
    {
        private bool isInited;
        private bool isQuestionCommentEnabled;

        protected bool IsQuestionCommentEnabled
        {
            get
            {
                if (!isInited)
                {
                    SPList configList = SPContext.Current.Web.GetListByUrl("ConfigurationList");
                    SPListItem config = configList.GetItemById(1);
                    isQuestionCommentEnabled = SPContext.Current.Web.IsCurrentUserMemberOfGroup(new SPFieldLookupValue(config["QuestionCommentGroup"].ToString()).LookupId);
                    isInited = true;
                }
                return isQuestionCommentEnabled;
            }
        }

        protected bool IsIssueEditAccessible
        {
            get { return _isIssueEditAccessibleLazy.Value; }
        }

        private readonly Lazy<bool> _isIssueEditAccessibleLazy = new Lazy<bool>(() =>
        {
            SPList meetingList = SPContext.Current.Web.GetListByUrl("IssueMVKList");
            SPBasePermissions permissionMask = meetingList.GetUserEffectivePermissions(SPContext.Current.Web.CurrentUser.LoginName);
            return (permissionMask & SPBasePermissions.EditListItems) != 0;
        });

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static string AddIssuesP(string meetingMvkId, string[] issuePIdList)
        {
            string returnMessage = string.Format("Веб-метод AddIssuesP(meetingMvkId = {0}, issuePIdList = [{1}]): ", meetingMvkId, string.Join(",", issuePIdList));
            try
            {
                int meetingMvkIdInt = Convert.ToInt32(meetingMvkId);
                BL.MeetingMvk.CreateIssuesMvkFromIssuesP(SPContext.Current.Web, meetingMvkIdInt, issuePIdList.Select(s => Convert.ToInt32(s)));
            }
            catch (Exception e)
            {
                return Log.Unexpected(e, returnMessage + "ошибка");
            }
            return returnMessage + "успешно выполнен";
        }
    }
}
