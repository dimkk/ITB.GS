using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            using (var site = new SPSite("http://gs.msk.mosreg.ru"))
            {
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

                SPList municipalityList = site.RootWeb.GetListByUrl("List1");
                ShowListUsages(municipalityList);
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

            do
            {
                SPListItemCollection targets = targetList.GetItems(query);
                foreach (SPListItem target in targets)
                {
                    foreach (string source in fieldsMapping.Keys)
                        target[fieldsMapping[source]] = target[source];

                    using (var scope = new DisabledItemEventsScope())
                        target.SystemUpdate();
                }
                query.ListItemCollectionPosition = targets.ListItemCollectionPosition;
            } while (query.ListItemCollectionPosition != null);
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
