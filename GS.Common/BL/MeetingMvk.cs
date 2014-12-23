using CamlexNET;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Common.BL
{
    public class MeetingMvk
    {
        protected static readonly Dictionary<string, string> IssuePToIssueMvkFieldMapping = new Dictionary<string, string>()
        {
            { "IssueAddressP",      "IssueAddressMVK" },
            { "IssueDescriptionP",  "IssueDescriptionMVK" },
            { "IssueBuilderP",      "IssueInvestorMVK" },
            { "IssueCadastreIdP",   "IssueCadastreIdMVK" },
            { "IssueCategoryP",     "IssueCategoryMVK" },
            { "IssueMunicipalityP", "IssueMunicipalDistrictMVK" },
            { "IssueSettlementP",   "IssueSettlementMVK" }
        };

        #region IssuesMvk
        public static void CreateIssuesMvkFromUnusedIssuesP(SPWeb web, int meetingMvkId)
        {
            SPQuery query =
                Camlex.Query()
                    .Where(x => x[IssueP.IssuePIssueMvkCountFieldName] == (DataTypes.Integer)"0" && x[IssueP.IssuePIssueGsCountFieldName] == (DataTypes.Integer)"0")
                    .OrderBy(x => x["Created"])
                    .ToSPQuery();
            query.RowLimit = 10;

            CreateIssuesMvkFromIssuesP(web, meetingMvkId, web.GetListItems(IssueP.IssuePListName, query));
        }

        public static void CreateIssuesMvkFromIssuesP(SPWeb web, int meetingMvkId, IEnumerable<int> issuePIdList)
        {
            SPQuery query =
                Camlex.Query()
                    .Where(x => issuePIdList.Contains((int)x["ID"]))
                    .ToSPQuery();

            CreateIssuesMvkFromIssuesP(web, meetingMvkId, web.GetListItems(IssueP.IssuePListName, query));
        }

        public static void DeleteIssuesMvk(SPWeb web, int meetingMvkId)
        {
            var exceptions = new List<Exception>();

            SPQuery query =
                Camlex.Query()
                    .Where(x => x[IssueMvk.IssueMeetingMvkFieldName] == (DataTypes.LookupId)meetingMvkId.ToString())
                    .ToSPQuery();

            List<SPListItem> issuesMvk = web.GetListItems(IssueMvk.IssueMvkListName, query).ToList();
            for (int i = issuesMvk.Count - 1; i >= 0; i--)
                try
                {
                    issuesMvk[i].Delete();
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception(string.Format("При обработке элемента (ID = {0}) списка {1} произошло неожиданное исключение", issuesMvk[i].ID, IssueMvk.IssueMvkListName), e));
                }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        internal static int GetIssueMvkNextNumber(SPWeb web, int meetingMvkId)
        {
            SPQuery query =
                Camlex.Query()
                    .Where(x => x[IssueMvk.IssueMeetingMvkFieldName] == (DataTypes.LookupId)meetingMvkId.ToString())
                    .OrderBy(o => o[IssueMvk.IssueNumberMvkFieldName] as Camlex.Desc)
                    .ToSPQuery();
            query.RowLimit = 1;

            SPListItem[] issuesMvk = web.GetListItems(IssueMvk.IssueMvkListName, query).ToArray();
            return issuesMvk.Length == 0 ? 1 : Convert.ToInt32(issuesMvk[0][IssueMvk.IssueNumberMvkFieldName]) + 1;
        }

        internal static void CreateIssuesMvkFromIssuesP(SPWeb web, int meetingMvkId, IEnumerable<SPListItem> issuesP)
        {
            var exceptions = new List<Exception>();
            int nextIssueMvkNumber = GetIssueMvkNextNumber(web, meetingMvkId);
            foreach (SPListItem issueP in issuesP)
            {
                try
                {
                    CreateIssueMvkFromIssueP(web, meetingMvkId, issueP, nextIssueMvkNumber);
                    nextIssueMvkNumber++;
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception(string.Format("При обработке элемента (ID = {0}) списка {1} произошло неожиданное исключение", issueP.ID, IssueP.IssuePListName), e));
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        internal static void CreateIssueMvkFromIssueP(SPWeb web, int meetingMvkId, SPListItem issueP, int issueMvkNumber)
        {
            SPList issueMvkList = web.GetListByUrl(IssueMvk.IssueMvkListName);

            SPListItem newIssueMvk = issueMvkList.AddItem();
            newIssueMvk[IssueMvk.IssueMeetingMvkFieldName] = new SPFieldLookupValue(meetingMvkId, meetingMvkId.ToString());
            newIssueMvk[IssueMvk.IssueIssuePFieldName] = new SPFieldLookupValue(issueP.ID, issueP.ID.ToString());
            newIssueMvk[IssueMvk.IssueNumberMvkFieldName] = issueMvkNumber;

            foreach (string issuePFieldName in IssuePToIssueMvkFieldMapping.Keys)
                newIssueMvk[IssuePToIssueMvkFieldMapping[issuePFieldName]] = issueP[issuePFieldName];

            web.AllowUnsafeUpdates = true;
            newIssueMvk.Update();
            web.AllowUnsafeUpdates = false;
        }
        #endregion
    }
}
