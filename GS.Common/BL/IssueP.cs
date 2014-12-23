using ITB.SP.Tools;
using Microsoft.SharePoint;

namespace GS.Common.BL
{
    public class IssueP
    {
        public static readonly string IssuePListName = "IssuePList";
        public static readonly string IssuePIssueGsCountFieldName = "IssuePIssueGsCount";
        public static readonly string IssuePIssueMvkCountFieldName = "IssuePIssueMvkCount";
        public static readonly string IssuePStatusFieldName = "IssueStatusP";
        public static readonly string IssuePSourceTypeFieldName = "IssueSourceTypeP";

        public static void SetStatus(SPListItem issueP, StatusEnum status)
        {
            issueP[IssuePStatusFieldName] = new SPFieldLookupValue(Status.GetIdByStatus(issueP.Web, status), null);
            issueP.Web.AllowUnsafeUpdates = true;
            issueP.Update();
            issueP.Web.AllowUnsafeUpdates = false;
        }

        public static StatusEnum GetStatus(SPListItem issueP)
        {
            int statusId = issueP.GetFieldLookup(IssuePStatusFieldName).LookupId;
            return Status.GetById(issueP.Web, statusId);
        }
    }
}
