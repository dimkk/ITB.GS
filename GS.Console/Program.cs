using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamlexNET;
using ITB.SP.Tools;
using Microsoft.SharePoint;

namespace GS.Console
{
    class Program
    {
        static Dictionary<string, string> issueMappingToTemp = new Dictionary<string, string>()
        {
            { "AgendaQuestionAddress", "AgendaQuestionAddress1" },
            { "AgendaQuestionExtResources", "AgendaQuestionExtResources1" },
            { "AgendaQuestionIncomingDate", "AgendaQuestionIncomingDate1" },
            //{ "AgendaQuestionReporter", "AgendaQuestionReporter1" },
            { "AgendaQuestionReporterFullNameLink", "AgendaQuestionReporter1" },
            { "MeetingLink", "MeetingLink1" },
            { "AgendaQuestionDeclarant", "AgendaQuestionDeclarant1" },
            { "AgendaQuestionInvestor", "AgendaQuestionInvestor1" },
            { "_x0418__x043d__x0444__x043e_", "AgendaQuestionInfo" },
            { "CadastreNumber", "CadastreNumber1" },
            { "QuestionCategoryLink", "QuestionCategoryLink1" },
            { "AgendaQuestionComment", "AgendaQuestionComment1" },
            { "IssueMunicipalityGs", "IssueMunicipalityGs1" },
            { "AgendaQuestionSiteName", "AgendaQuestionSiteName1" },
            { "AgendaQuestionNumber", "AgendaQuestionNumber1" },
            { "AgendaQuestionDescription", "AgendaQuestionDescription1" },
            { "AgendaQuestionReason", "AgendaQuestionReason1" },
            { "IssueGsIssueP", "IssueGsIssueP1" },
            { "IssueSettlementGs", "IssueSettlementGs1" },
            { "AgendaQuestionIsConsidered", "AgendaQuestionIsConsidered1" },
            { "AgendaQuestionProtocolDecision", "AgendaQuestionProtocolDecision1" },
            { "AgendaLinkedQuestionLink", "AgendaLinkedQuestionLink1" },
            //{ "AgendaQuestionCoreporter", "AgendaQuestionCoreporter1" },
            { "AgendaQuestionSoreporterFullNameLink", "AgendaQuestionCoreporter1" },
            { "AgendaQuestionTheme", "AgendaQuestionTheme1" },
            { "_x0422__x0438__x043f__x0020__x04", "AgendaQuestionObjectType" },
            { "AgendaQuestionProjectType", "AgendaQuestionProjectType1" },
            { "AgendaQuestionDecisionType", "AgendaQuestionDecisionType1" }
        };

        static void Main(string[] args)
        {
            string url = "http://sp2013dev:8081/";
            //CleanAttachments(url, "Вложения отчета по поручению");
            //CleanOldVersions(url, "Вложения наборы вопроса повестки");
            //AddUsersToGroup(url, "users.csv", "ignored.csv", "ДТП - ОМСУ");

            using (var site = new SPSite(url))
            {
                DeleteLandLists(site);

                //SPList issueList = site.RootWeb.GetListByUrl("AgendaQuestionList");
                //var issueMappingFromTemp = new Dictionary<string, string>();
                //foreach (string key in issueMappingToTemp.Values)
                //    issueMappingFromTemp.Add(key, issueMappingToTemp.Keys.Where(s => issueMappingToTemp[s] == key).Single());

                //CopyFields(issueList, issueMappingToTemp);
                //SetContentType(issueList, "0x01003C18ED48E7474D6EA2943047D32504BE0034919DBB6BDF62458C7ACB53AB30C2A6");
                //CopyFields(issueList, issueMappingFromTemp);

                //SPList oldMunicipalityList = site.RootWeb.GetListByUrl("List1");
                //SPList oldSettlementList = site.RootWeb.GetListByUrl("List");
                //SPList newMunicipalityList = site.RootWeb.GetListByUrl("Municipality");

                //var municipalityMapping = GetMunicipalityMapping(oldMunicipalityList, newMunicipalityList);
                //var settlementMapping = GetMunicipalityMapping(oldSettlementList, newMunicipalityList);

                //SPList issueGsList = site.RootWeb.GetListByUrl("AgendaQuestionList");
                //ConvertMunicipalities(issueGsList, municipalityMapping, settlementMapping, "_x041c__x0443__x043d__x0438__x04", "_x041f__x043e__x0441__x0435__x04", "IssueMunicipalityGs", "IssueSettlementGs");

                //SPList issueMvkList = site.RootWeb.GetListByUrl("IssueMVKList");
                //ConvertMunicipalities(issueMvkList, municipalityMapping, settlementMapping, "IssueMunicipalDistrictMVK", "IssueSettlementMVK", "MunicipalityMvk", "SettlementMvk");

                //CopyFields(issueMvkList, new Dictionary<string, string>() { { "MunicipalityMvk", "IssueMunicipalDistrictMVK" }, { "SettlementMvk", "IssueSettlementMVK" } });

                //SPList municipalityList = site.RootWeb.GetListByUrl("List1");
                //ShowListUsages(municipalityList);

            }

            System.Console.WriteLine("Завершено");
            System.Console.ReadLine();
        }

        private static void DeleteLandLists(SPSite site)
        {
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["ReportLand"]);
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["AssignmentLand"]);
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["IssueAttachmentLand"]);
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["MeetingAttachmentLand"]);
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["IssueLand"]);
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["MeetingLand"]);
            DeleteListsByContentType(site.RootWeb.AvailableContentTypes["IssueCategoryLand"]);
        }

        private static void DeleteListsByContentType(SPContentType contentType)
        {
            if (contentType == null)
                return;

            foreach (SPContentTypeUsage usage in SPContentTypeUsage.GetUsages(contentType).Where(s => s.IsUrlToList))
            {
                SPList list = contentType.ParentWeb.GetList(usage.Url);
                list.Delete();
            }
        }

        private static void ShowListUsages(SPList targetList)
        {
            foreach (SPList list in targetList.ParentWeb.Lists)
            {
                var fields = new List<string>();
                foreach (var lookup in list.Fields.OfType<SPFieldLookup>())
                    if (lookup.LookupList.Trim('{', '}') == targetList.ID.ToString())
                        fields.Add(lookup.Title + "\t" + lookup.InternalName);

                if (fields.Count > 0)
                {
                    System.Console.WriteLine(list.Title);
                    foreach (var field in fields)
                        System.Console.WriteLine("\t" + field);
                }
            }
        }

        private static void ConvertMunicipalities(SPList targetList, Dictionary<int, int> municipalityMapping, Dictionary<int, int> settlementMapping, string oldMunicipalityFieldName, string oldSettlementFieldName, string newMunicipalityFieldName, string newSettlementFieldName)
        {
            SPQuery query = Camlex.Query().ToSPQuery();
            query.RowLimit = 1000;

            do
            {
                SPListItemCollection targets = targetList.GetItems(query);
                foreach (SPListItem target in targets)
                {
                    bool isChanged = false;
                    SPFieldLookupValue oldMunicipalityLookup = target.GetFieldLookup(oldMunicipalityFieldName);
                    if (oldMunicipalityLookup.LookupId > 0)
                    {
                        System.Console.WriteLine(oldMunicipalityLookup.LookupValue);
                        if (target.GetFieldLookup(newMunicipalityFieldName).LookupId != municipalityMapping[oldMunicipalityLookup.LookupId])
                        {
                            target[newMunicipalityFieldName] = new SPFieldLookupValue(municipalityMapping[oldMunicipalityLookup.LookupId], null);
                            isChanged = true;
                        }
                    }

                    SPFieldLookupValue oldSettlementLookup = target.GetFieldLookup(oldSettlementFieldName);
                    if (oldSettlementLookup.LookupId > 0)
                    {
                        System.Console.WriteLine(oldSettlementLookup.LookupValue);
                        if (target.GetFieldLookup(newSettlementFieldName).LookupId != settlementMapping[oldSettlementLookup.LookupId])
                        {
                            target[newSettlementFieldName] = new SPFieldLookupValue(settlementMapping[oldSettlementLookup.LookupId], null);
                            isChanged = true;
                        }
                    }

                    if (isChanged)
                        using (var scope = new DisabledItemEventsScope())
                            target.SystemUpdate();
                }
                query.ListItemCollectionPosition = targets.ListItemCollectionPosition;
            } while (query.ListItemCollectionPosition != null);
        }

        private static Dictionary<int, int> GetMunicipalityMapping(SPList oldMunicipalityList, SPList newMunicipalityList)
        {
            var oldOkatoMapping = new Dictionary<int, string>();
            var oldTitleMapping = new Dictionary<string, string>();
            foreach (SPListItem item in oldMunicipalityList.GetItems(Camlex.Query().ToSPQuery()))
            {
                var okato = item.GetFieldValue<string>("_x041a__x043e__x0434__x0020__x04");
                oldOkatoMapping.Add(item.ID, okato);
                oldTitleMapping.Add(okato, item.Title);
            }

            var newOkatoMapping = new Dictionary<string, int>();
            foreach (SPListItem item in newMunicipalityList.GetItems(Camlex.Query().ToSPQuery()))
            {
                var okato = item.GetFieldValue<string>("MunicipalityOkato");
                if (oldTitleMapping.ContainsKey(okato))
                {
                    newOkatoMapping.Add(okato, item.ID);
                    string type = item.GetFieldValue<string>("MunicipalityType").ToLower();
                    string newName = item.Title.ToLower().Replace(type, string.Empty).Trim().Replace('ё', 'е');
                    string oldName = oldTitleMapping[okato].ToLower().Replace(type, string.Empty).Trim().Replace('ё', 'е');
                    if (newName != oldName)
                        System.Console.WriteLine("{0} != {1}", oldName, newName);
                }
            }

            return oldOkatoMapping.Keys.ToDictionary(id => id, id => newOkatoMapping[oldOkatoMapping[id]]);
        }

        private static void CopyFields(SPList targetList, Dictionary<string, string> fieldsMapping)
        {
            SPQuery query = Camlex.Query().ToSPQuery();
            query.RowLimit = 1000;
            int count = 0;
            do
            {
                SPListItemCollection targets = targetList.GetItems(query);
                foreach (SPListItem target in targets)
                {
                    foreach (string source in fieldsMapping.Keys)
                    {
                        target[fieldsMapping[source]] = target[source];
                    }

                    using (var scope = new DisabledItemEventsScope())
                        target.SystemUpdate();

                    System.Console.WriteLine(++count);
                }
                query.ListItemCollectionPosition = targets.ListItemCollectionPosition;
            } while (query.ListItemCollectionPosition != null);
        }

        private static void SetContentType(SPList targetList, string contentTypeId)
        {
            SPQuery query = Camlex.Query().ToSPQuery();
            query.RowLimit = 1000;
            int count = 0;
            do
            {
                SPListItemCollection targets = targetList.GetItems(query);
                foreach (SPListItem target in targets)
                {
                    target["ContentTypeId"] = contentTypeId;

                    using (var scope = new DisabledItemEventsScope())
                        target.SystemUpdate();

                    System.Console.WriteLine(++count);
                }
                query.ListItemCollectionPosition = targets.ListItemCollectionPosition;
            } while (query.ListItemCollectionPosition != null);
        }


        public static void CleanOldVersions(string siteUrl, string libraryTitle)
        {
            int counter = 0;
            using (var site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                using (var fileLog = new StreamWriter("log.txt", false))
                {
                    SPList list = web.Lists[libraryTitle];
                    foreach (SPListItem item in list.Items)
                    {
                        if (item.ContentType.Name != "Набор документов")
                        {

                        }
                        SPListItemVersionCollection versions = item.Versions;
                        int count = versions.Count;
                        int deleteCount = 0;
                        for (int i = versions.Count - 1; i > 0; i--)
                        {
                            if (versions[i].VersionId != versions[0].VersionId)
                            {
                                try
                                {
                                    versions[i].Delete();
                                    deleteCount++;
                                }
                                catch (Exception ex)
                                {
                                    fileLog.WriteLine("Ошибка при удалении версии {0} файла {1}:\r\n{2}", i, item.File.ToString(), ex.ToString());
                                }
                            }
                        }
                        if (count > 1)
                            fileLog.WriteLine("Удалено {0} из {1} версий файла {2}", deleteCount, count, item.File.ToString());

                        System.Console.WriteLine(++counter);
                    }
                    fileLog.Flush();
                }
            }
        }

        public static void CleanAttachments(string siteUrl, string libraryTitle)
        {
            using (var site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[libraryTitle];
                    for (int i = list.Items.Count - 1; i >= 0; i--)
                    {
                        System.Console.WriteLine(i);
                        SPListItem item = list.Items[i];
                        if (item.ContentType.Name != "Набор документов")
                        {
                            System.Console.WriteLine(item.Name);
                            item.Delete();
                        }
                    }
                }
            }
        }

        private static void AddUsersToGroup(string targetUrl, string inFile, string outFile, string groupName)
        {
            using (var usersStream = new StreamReader(inFile))
            using (var outStream = new StreamWriter(outFile))
            using (var site = new SPSite(targetUrl))
            {
                var group = site.RootWeb.Groups.GetByName(groupName);
                while (!usersStream.EndOfStream)
                {
                    string line = usersStream.ReadLine();
                    int index = line.IndexOf(';');
                    if (index < 0 || line.IndexOf(';', index + 1) < 0 || line[0] == '"')
                        continue;

                    string name = line.Substring(0, index);
                    List<SPUser> allUsers = site.RootWeb.AllUsers.Cast<SPUser>().Where(s => s.Name == name).ToList();
                    SPUser user = allUsers.SingleOrDefault(s => s.LoginName.ToLower().Contains("fbamembershipprovider"));

                    if (user == null)
                        outStream.WriteLine(line);
                    else
                    {
                        if (allUsers.Count > 1)
                        {
                        }
                        group.AddUser(user);
                        System.Console.WriteLine(user.Name);
                    }
                }
            }
        }
    }

    public class DisabledItemEventsScope : SPItemEventReceiver, IDisposable
    {
        public DisabledItemEventsScope()
        {
            base.DisableEventFiring();
        }

        #region IDisposable Members

        public void Dispose()
        {
            base.EnableEventFiring();
        }

        #endregion
    }
}
