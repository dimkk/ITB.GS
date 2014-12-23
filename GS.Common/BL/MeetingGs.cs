using CamlexNET;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Common.BL
{
    public class MeetingGs
    {
        protected static readonly Dictionary<string, string> IssuePToIssueGsFieldMapping = new Dictionary<string, string>()
        {
            { "IssueAddressP",      "AgendaQuestionAddress" },
            { "IssueDescriptionP",  "AgendaQuestionDescription" },
            { "IssueBuilderP",      "AgendaQuestionInvestor" },
            { "IssueCadastreIdP",   "CadastreNumber" },
            { "IssueCategoryP",     "QuestionCategoryLink" },
            { "IssueMunicipalityP", "IssueMunicipalityGs" },
            { "IssueSettlementP",   "IssueSettlementGs" }
        };

        #region IssuesGs
        public static void CreateIssuesGsFromUnusedIssuesP(SPWeb web, int meetingGsId)
        {
            SPQuery query =
                Camlex.Query()
                    .Where(x => x[IssueP.IssuePIssueMvkCountFieldName] == (DataTypes.Integer)"0" && x[IssueP.IssuePIssueGsCountFieldName] == (DataTypes.Integer)"0")
                    .OrderBy(x => x["Created"])
                    .ToSPQuery();
            query.RowLimit = 10;

            CreateIssuesGsFromIssuesP(web, meetingGsId, web.GetListItems(IssueP.IssuePListName, query));
        }

        public static void CreateIssuesGsFromIssuesP(SPWeb web, int meetingGsId, IEnumerable<int> issuePIdList)
        {
            SPQuery query =
                Camlex.Query()
                    .Where(x => issuePIdList.Contains((int)x["ID"]))
                    .ToSPQuery();

            CreateIssuesGsFromIssuesP(web, meetingGsId, web.GetListItems(IssueP.IssuePListName, query));
        }

        public static void DeleteIssuesGs(SPWeb web, int meetingGsId)
        {
            var exceptions = new List<Exception>();

            SPQuery query =
                Camlex.Query()
                    .Where(x => x[IssueGs.IssueMeetingGsFieldName] == (DataTypes.LookupId)meetingGsId.ToString())
                    .ToSPQuery();

            List<SPListItem> issuesGs = web.GetListItems(IssueGs.IssueGsListName, query).ToList();
            for (int i = issuesGs.Count - 1; i >= 0; i--)
                try
                {
                    issuesGs[i].Delete();
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception(string.Format("При обработке элемента (ID = {0}) списка {1} произошло неожиданное исключение", issuesGs[i].ID, IssueGs.IssueGsListName), e));
                }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        internal static int GetIssueGsNextNumber(SPWeb web, int meetingGsId)
        {
            SPQuery query =
                Camlex.Query()
                    .Where(x => x[IssueGs.IssueMeetingGsFieldName] == (DataTypes.LookupId)meetingGsId.ToString())
                    .OrderBy(o => o[IssueGs.IssueNumberGsFieldName] as Camlex.Desc)
                    .ToSPQuery();
            query.RowLimit = 1;

            SPListItem[] issuesGs = web.GetListItems(IssueGs.IssueGsListName, query).ToArray();
            return issuesGs.Length == 0 ? 1 : Convert.ToInt32(issuesGs[0][IssueGs.IssueNumberGsFieldName]) + 1;
        }

        internal static void CreateIssuesGsFromIssuesP(SPWeb web, int meetingGsId, IEnumerable<SPListItem> issuesP)
        {
            var exceptions = new List<Exception>();
            int nextIssueGsNumber = GetIssueGsNextNumber(web, meetingGsId);
            foreach (SPListItem issueP in issuesP)
            {
                try
                {
                    CreateIssueGsFromIssueP(web, meetingGsId, issueP, nextIssueGsNumber);
                    nextIssueGsNumber++;
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception(string.Format("При обработке элемента (ID = {0}) списка {1} произошло неожиданное исключение", issueP.ID, IssueP.IssuePListName), e));
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        internal static void CreateIssueGsFromIssueP(SPWeb web, int meetingGsId, SPListItem issueP, int issueGsNumber)
        {
            SPList issueGsList = web.GetListByUrl(IssueGs.IssueGsListName);

            SPListItem newIssueGs = issueGsList.AddItem();
            newIssueGs[IssueGs.IssueMeetingGsFieldName] = new SPFieldLookupValue(meetingGsId, null);
            newIssueGs[IssueGs.IssueGsIssuePFieldName] = new SPFieldLookupValue(issueP.ID, null);
            newIssueGs[IssueGs.IssueNumberGsFieldName] = issueGsNumber;

            foreach (string issuePFieldName in IssuePToIssueGsFieldMapping.Keys)
                newIssueGs[IssuePToIssueGsFieldMapping[issuePFieldName]] = issueP[issuePFieldName];

            web.AllowUnsafeUpdates = true;
            newIssueGs.Update();
            web.AllowUnsafeUpdates = false;
        }
        #endregion

    }
}
