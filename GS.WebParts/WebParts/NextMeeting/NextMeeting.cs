using Microsoft.SharePoint.Security;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;

namespace GS.WebParts
{

    /// <summary>
    /// TODO: Add comment for webpart NextMeeting
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class NextMeeting : System.Web.UI.WebControls.WebParts.WebPart
    {
        private const string AscxPath = @"/_CONTROLTEMPLATES/15/GS/NextMeetingUserControl.ascx";

        #region Properties
        [Category("Настройки"), WebBrowsable, Personalizable(PersonalizationScope.Shared), FriendlyName("Имя списка заседаний")]
        public string MeetingsListName { get; set; }
        [Category("Настройки"), WebBrowsable, Personalizable(PersonalizationScope.Shared), FriendlyName("Имя поля статуса заседания")]
        public string MeetingStatusFieldName { get; set; }
        [Category("Настройки"), WebBrowsable, Personalizable(PersonalizationScope.Shared), FriendlyName("Имя поля даты заседания")]
        public string MeetingDateFieldName { get; set; }
        [Category("Настройки"), WebBrowsable, Personalizable(PersonalizationScope.Shared), FriendlyName("Имя поля номера заседания")]
        public string MeetingNumberFieldName { get; set; }
        [Category("Настройки"), WebBrowsable, Personalizable(PersonalizationScope.Shared), FriendlyName("Имя поля места заседания")]
        public string MeetingPlaceFieldName { get; set; }
        [Category("Настройки"), WebBrowsable, Personalizable(PersonalizationScope.Shared), FriendlyName("Фильтр названия заседания")]
        public string MeetingTitleFilter { get; set; }
        #endregion

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(AscxPath);
            InitControl(control);
            Controls.Add(control);
        }
        protected void InitControl(Control control)
        {
            var uc = (NextMeetingUserControl)control;
            
            uc.Title = Title;
            uc.MeetingListName = MeetingsListName;
            uc.MeetingNumberFieldName = MeetingNumberFieldName;
            uc.MeetingDateFieldName = MeetingDateFieldName;
            uc.MeetingStatusFieldName = MeetingStatusFieldName;
            uc.MeetingPlaceFieldName = MeetingPlaceFieldName;
            uc.MeetingTitleFilter = MeetingTitleFilter;
        }
    }
}

