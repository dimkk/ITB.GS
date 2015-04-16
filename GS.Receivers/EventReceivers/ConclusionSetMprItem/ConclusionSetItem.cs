using ITB.SP.Tools;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace GS.Mpr.Receivers
{
    public class ConclusionSetItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            TryFillConclusionText(properties);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            TryFillConclusionText(properties);
            EventFiringEnabled = true;
        }

        #region Actions

        public readonly static string ApplicationMprListName = "ReestrDTP";
        public readonly static string ApplicationConclusionsMprFieldName = "ApplicationConclusionTextMpr";
        public readonly static string ConclusionApplicationMprFieldName = "ConclusionApplicationMpr";

        private void TryFillConclusionText(SPItemEventProperties properties1)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                string url = !string.IsNullOrEmpty(properties1.BeforeUrl) ? properties1.BeforeUrl : properties1.AfterUrl;
                if (!SPUtility.GetUrlFileName(url).Contains('.'))
                    return;

                string folderUrl = SPUtility.GetUrlDirectory(url);
                SPFolder folder;
                int appId = 0;
                using (var site = new SPSite(properties1.SiteId))
                using (var web = site.OpenWeb(properties1.Web.ID))
                {
                    if (SPFolderHierarchy.TryGetFolderByUrl(web, folderUrl, out folder))
                    {
                        try
                        {
                            var sb = new StringBuilder();
                            foreach (var file in folder.Files.OfType<SPFile>().OrderBy(s => s.TimeCreated))
                                sb.AppendFormat("<a href=\"/{0}\" target=\"_blank\">{1}<a><br/>", file.Url, file.Name);

                            appId = folder.Item.GetFieldLookup(ConclusionApplicationMprFieldName).LookupId;
                            SPList appList = web.GetListByUrl(ApplicationMprListName);
                            SPListItem app = appList.GetItemById(appId);
                            app[ApplicationConclusionsMprFieldName] = sb.ToString();
                            app.SystemUpdate();
                        }
                        catch (Exception e)
                        {
                            Log.Unexpected(e,
                                "Неожиданное исключение при изменении поля {0} элемента (ID = {1}) списка {2} для набора документов (ID = {3}) библиотеки {4}",
                                ApplicationConclusionsMprFieldName, appId, ApplicationMprListName, folder.Item.ID,
                                folder.Url);
                        }
                    }
                }
            });
        }
        #endregion
    }
}

