using CamlexNET;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Linq;

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
        #endregion

        #region Calculated Data for View
        protected bool IsNextMeeting { get; private set; }

        protected string MeetingNumber { get; private set; }

        protected DateTime MeetingDate { get; private set; }

        protected string MeetingPlace { get; private set; }

        protected string MeetingUrl { get; private set; }

        protected string ErrorMessage { get; private set; }
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                SPQuery query =
                    Camlex.Query()
                        .Where(x => x[MeetingStatusFieldName] == (DataTypes.Choice)"Планируемое" || x[MeetingStatusFieldName] == (DataTypes.Choice)"Planning")
                        .OrderBy(o => o[MeetingDateFieldName] as Camlex.Asc)
                        .ToSPQuery();
                query.RowLimit = 1;

                SPListItem meeting = SPContext.Current.Web.GetListItems(MeetingListName, query).FirstOrDefault();
                IsNextMeeting = meeting != null;

                if (IsNextMeeting)
                {
                    MeetingNumber = meeting.GetFieldValue<string>(MeetingNumberFieldName);
                    MeetingDate = meeting.GetFieldValue<DateTime>(MeetingDateFieldName);
                    MeetingPlace = meeting.GetFieldValue<string>(MeetingPlaceFieldName);
                    MeetingUrl = meeting.GetDisplayUrl();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = Log.Unexpected(ex, "При получении деталей планируемого заседания {0} произошла неожиданная ошибка", MeetingListName);
            }
        }
    }
}

