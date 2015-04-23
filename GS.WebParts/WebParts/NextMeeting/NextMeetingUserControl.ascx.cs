using System.Collections.Generic;
using System.Linq.Expressions;
using CamlexNET;
using CamlexNET.Interfaces;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Linq;
using System.Web;

namespace GS.WebParts
{
    public partial class NextMeetingUserControl : System.Web.UI.UserControl
    {
        #region Parameters
        public string Title { get; set; }

        public string MeetingListName { get; set; }

        public string MeetingStatusFieldName { get; set; }

        public string MeetingDateFieldName { get; set; }

        public string MeetingNumberFieldName { get; set; }

        public string MeetingPlaceFieldName { get; set; }

        public string MeetingTitleFilter { get; set; }

        public string BackgroundColor { get; set; }
        #endregion

        #region Calculated Data for View
        protected bool IsNextMeeting { get; private set; }

        protected string MeetingNumber { get; private set; }

        protected DateTime MeetingDate { get; private set; }

        protected string MeetingPlace { get; private set; }

        protected string MeetingUrl { get; private set; }

        protected string ErrorMessage { get; private set; }

        protected string ListUrl { get; private set; }

        protected string PanelStyle
        {
            get
            {
                if (string.IsNullOrEmpty(BackgroundColor))
                    return string.Empty;

                return string.Format("background-color:#{0}", BackgroundColor.Replace("#", string.Empty));
            }
        }
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                var expressions = new List<Expression<Func<SPListItem, bool>>>
                {
                    x =>
                        x[MeetingStatusFieldName] == (DataTypes.Choice) "Планируемое" ||
                        x[MeetingStatusFieldName] == (DataTypes.Choice) "Planning"
                };
                
                if (!string.IsNullOrEmpty(MeetingTitleFilter))
                    expressions.Add(x => ((string)x["Title"]).Contains(MeetingTitleFilter));

                SPQuery query =
                    Camlex.Query().WhereAll(expressions).OrderBy(o => o[MeetingDateFieldName] as Camlex.Asc).ToSPQuery();

                query.RowLimit = 1;

                SPList meetingList = SPContext.Current.Web.GetListByUrl(MeetingListName);
                SPListItem meeting = meetingList.GetItems(query).Cast<SPListItem>().FirstOrDefault();
                IsNextMeeting = meeting != null;

                ListUrl = meetingList.RootFolder.ServerRelativeUrl;
                if (IsNextMeeting)
                {
                    MeetingNumber = meeting.GetFieldValue<string>(MeetingNumberFieldName);
                    MeetingDate = meeting.GetFieldValue<DateTime>(MeetingDateFieldName);
                    MeetingPlace = meeting.GetFieldValue<string>(MeetingPlaceFieldName);
                    MeetingUrl = meeting.GetDisplayUrl().AddUrlParam("Source", HttpContext.Current.Request.Url.ToString());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = Log.Unexpected(ex, "При получении деталей планируемого заседания {0} произошла неожиданная ошибка", MeetingListName);
            }
        }
    }
}

