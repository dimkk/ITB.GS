using CamlexNET;
using ITB.SP.Tools;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System;
using System.Collections;
using System.Linq;
using System.Security.Permissions;

namespace GS.Mpr.Receivers
{
    public class ApplicationItem : SPItemEventReceiver
    {
        #region Events
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            CreateDocumentSet(properties.ListItem);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            DeleteDocumentSet(properties.List, properties.ListItemId);
            EventFiringEnabled = true;
        }
        #endregion

        #region Actions

        public readonly static string ConclusionMprListName = "ConclusionSetMpr";
        public readonly static string ConclusionApplicationMprFieldName = "ConclusionApplicationMpr";
        public readonly static string ApplicationNumberMprFieldName = "DtpNumber";

        public static void CreateDocumentSet(SPListItem listItem)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                try
                {
                    using (var site = new SPSite(listItem.Web.Site.ID))
                    using (var web = site.OpenWeb(listItem.Web.ID))
                    {
                        SPList documentSetList = web.GetListByUrl(ConclusionMprListName);
                        var name = string.Format("Материал №{0}",
                            listItem.GetFieldValue<string>(ApplicationNumberMprFieldName));
                        SPFolder folder = SPFolderHierarchy.GetSubFolder(documentSetList.RootFolder, name, false);

                        if (folder != null && folder.Exists)
                            return;

                        var props = new Hashtable
                        {
                            {
                                "DocumentSetDescription",
                                "Набор документов заключений материалов предварительного рассмотрения"
                            },
                            {ConclusionApplicationMprFieldName, listItem}
                        };
                        SPContentTypeId contentTypeId =
                            documentSetList.ContentTypes.OfType<SPContentType>()
                                .Single(s => s.Id.ToString().StartsWith(SPBuiltInContentTypeId.DocumentSet.ToString()))
                                .Id;
                        DocumentSet.Create(documentSetList.RootFolder, name, contentTypeId, props, true);
                    }
                }
                catch (Exception e)
                {
                    Log.Unexpected(e, "Неожиданная ошибка при создании набора документов элемента (ID = {0}) списка {1}",
                        listItem.ID, listItem.ParentList.RootFolder);
                }
            });
        }

        public static void DeleteDocumentSet(SPList list, int listItemId)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                int documentSetId = 0;
                try
                {
                    using (var site = new SPSite(list.ParentWeb.Site.ID))
                    using (var web = site.OpenWeb(list.ParentWeb.ID))
                    {
                        SPQuery query =
                            Camlex.Query()
                                .Where(
                                    x => x[ConclusionApplicationMprFieldName] == (DataTypes.LookupId)listItemId.ToString())
                                .ToSPQuery();
                        query.RowLimit = 1;

                        SPListItem item = web.GetListItems(ConclusionMprListName, query).FirstOrDefault();
                        if (item != null)
                        {
                            documentSetId = item.ID;
                            item.Delete();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Unexpected(e,
                        "Неожиданная ошибка при удалении набора документов (ID = {0}) библиотеки {1} для элемента (ID = {2}) списка {3}",
                        documentSetId, ConclusionMprListName, listItemId, list.RootFolder);
                }
            });
        }
        #endregion
    }
}

