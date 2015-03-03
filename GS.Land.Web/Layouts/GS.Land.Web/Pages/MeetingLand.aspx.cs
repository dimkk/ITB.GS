using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Linq;
using System.Web.Services;
//using BL = SAMRT.Common.BL;

namespace GS.Land.Web
{
    public partial class MeetingLand : LayoutsPageBase
    {
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
        public static string AddOrdersRg(string meetingRgId, string[] orderRgIdList)
        {
            string returnMessage = string.Format("Веб-метод AddOrdersRg(meetingRgId = {0}, orderRgIdList = [{1}]): ", meetingRgId, string.Join(",", orderRgIdList));
            try
            {
                int meetingRgIdInt = Convert.ToInt32(meetingRgId);
                //BL.MeetingRg.CreateIssuesRgFromOrdersRg(SPContext.Current.Web, meetingRgIdInt, orderRgIdList.Select(s => Convert.ToInt32(s)));
            }
            catch (Exception e)
            {
                return Log.Unexpected(e, returnMessage + "ошибка");
            }
            return returnMessage + "успешно выполнен";
        }
    }
}
