using GS.Common.BL;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System;
using System.Security.Permissions;

namespace GS.Receivers
{
    public class AssignmentItem : SPItemEventReceiver
    {
        protected readonly string NumberTemplate = "{0}.{1}";
        protected readonly string InfoTemplate = "Заседание №{0} от {1}, Вопрос №{2}";

        #region Events
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdding(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            SetNumberAndInfo(properties);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            SetNumberAndInfo(properties);
            EventFiringEnabled = true;
        }
        #endregion

        #region Actions
        private void SetNumberAndInfo(SPItemEventProperties properties)
        {
            try
            {
                int issueId;
                if (int.TryParse(Convert.ToString(properties.AfterProperties["AgendaQuestionLink"]), out issueId) && issueId > 0)
                {
                    SPListItem issue = properties.Web.GetListByUrl("AgendaQuestionList").GetItemById(issueId);
                    properties.AfterProperties["_x2116__x0020__x0440__x0435__x04"] = string.Format(NumberTemplate, issue[IssueGs.IssueNumberGsFieldName], properties.AfterProperties["AssignmentNumber"]);
                    int meetingId = issue.GetFieldLookup(IssueGs.IssueMeetingGsFieldName).LookupId;
                    SPListItem meeting = properties.Web.GetListByUrl("MeetingList").GetItemById(meetingId);
                    properties.AfterProperties["_x0418__x043d__x0444__x043e_"] = string.Format(InfoTemplate, meeting["MeetingNumber"], meeting.GetFieldValue<DateTime>("MeetingDate").ToShortDateString(), issue[IssueGs.IssueNumberGsFieldName]);
                }
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "При установке поля с номером или информацией поручения (ID = {0}) произошло неожиданное исключение", properties.AfterProperties["ID"]);
            }
        }
        #endregion
    }
}

