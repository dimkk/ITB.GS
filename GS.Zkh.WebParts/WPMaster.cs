//Sergey Mikolaytis
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebPartPages;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

namespace GS.Zkh.WebParts
{
    /// <summary>
    /// Web Part Tools: Attach, Detach, Filter etc.
    /// </summary>true
    internal static class WPMaster
    {
        /// <summary>
        /// Attach a XsltListViewWebPart to the specifiend page
        /// </summary>
        /// <param name="web">SharePoint Web</param>
        /// <param name="pageUrl">Relative Page Url</param>
        /// <param name="zoneID">Page WebPartZone ID</param>
        /// <param name="listUrl">Url to the List of ContentType</param>
        internal static XsltListViewWebPart AttachListView(SPWeb web, string pageUrl, string zoneID, string listUrl,
            SPLimitedWebPartManager webParts = null)
        {
            //Connect to the page WebPartManager
            if (webParts == null) webParts = web.GetLimitedWebPartManager(pageUrl, PersonalizationScope.Shared);

            //Get List and its DefaultView
            var list = web.GetList(listUrl);
            var view = list.DefaultView;

            //SetViewStyle(web, listUrl, 0);

            //Init Standard WebPart
            var lvwp = new XsltListViewWebPart
            {
                ListId = list.ID,
                ViewGuid = view.ID.ToString(),
                IsClientRender = false,
                ServerRender = true,
                AllowClose = false,
                AllowConnect = true,
                AllowEdit = true,
                AllowHide = true,
                AllowMinimize = true,
                AllowZoneChange = true,
                ClientRender = false,
                AsyncRefresh = true,
                ManualRefresh = true,
                AutoRefresh = false,
                ExportMode = WebPartExportMode.NonSensitiveData,
                Description = list.Description,
                Title = list.Title,
                ChromeType = PartChromeType.None
            };

            //Add webpart to the page
            webParts.AddWebPart(lvwp, zoneID, 1);
            webParts.SaveChanges(lvwp);

            return lvwp;
        }
        internal static void SetViewStyle(SPWeb web, string listUrl, int style)
        {
            var list = web.GetList(listUrl);
            var view = list.DefaultView;
            for (int i = 0; i < web.ViewStyles.Count; i++)
                if (web.ViewStyles[i].ID == style)
                    view.ApplyStyle(web.ViewStyles[i]);
            view.Update();
        }
        /// <summary>
        /// Detach all XsltListViewWebPart from the page
        /// </summary>
        /// <param name="web">SharePoint Web</param>
        /// <param name="pageUrl">Relative Page Url</param>
        internal static void DetachListViews(SPWeb web, string pageUrl, string zoneID)
        {
            //Connect to the page WebPartManager
            var webParts = web.GetLimitedWebPartManager(pageUrl, PersonalizationScope.Shared);
            //Delete all XsltListViewWebPart WebParts
            for (int i = 0; i < webParts.WebParts.Count; i++)
            {
                var wp = webParts.WebParts[i] as Microsoft.SharePoint.WebPartPages.XsltListViewWebPart;
                if (wp != null) webParts.DeleteWebPart(wp);
            }
        }

        /// <summary>
        /// Connects two webparts on the specified page
        /// </summary>
        /// <param name="web">SharePoint Web</param>
        /// <param name="pageUrl">Relative Page Url</param>
        /// <param name="providerID">Provider WebPart ID GUID String</param>
        /// <param name="providerField">Provider Connection Field Name</param>
        /// <param name="consumer">Consumer WebPart</param>
        /// <param name="consumerField">Consumer Connection Field Name</param>
        internal static void ConnectWebParts(SPWeb web, string pageUrl, string providerID, string providerField,
            System.Web.UI.WebControls.WebParts.WebPart consumer, string consumerField, SPLimitedWebPartManager webParts = null)
        {
            //Connect to the page WebPartManager
            if (webParts == null) webParts = web.GetLimitedWebPartManager(pageUrl, PersonalizationScope.Shared);

            //Get provider WebPart
            var provider = webParts.WebParts[providerID];

            //Create Connection Objects
            var providerConnectionPoint = webParts.GetProviderConnectionPoints(provider)["ListFormRowProvider_WPQ_"];
            var consumerConnectionPoint = webParts.GetConsumerConnectionPoints(consumer)["DFWP Filter Consumer ID"];
            var webPartTransformer = new SPRowToParametersTransformer()
            {
                ConsumerFieldNames = new string[] { consumerField },
                ProviderFieldNames = new string[] { providerField }
            };

            //AddConnection
            webParts.SPConnectWebParts(provider, providerConnectionPoint, consumer, consumerConnectionPoint, webPartTransformer);
        }

        /// <summary>
        /// Attach a XsltListViewWebPart to the specifiend page and connect it with existing WebPart
        /// </summary>
        /// <param name="web">SharePoint Web</param>
        /// <param name="pageUrl">Relative Page Url</param>
        /// <param name="zoneID">Page WebPartZone ID</param>
        /// <param name="listUrl">Url to the List of ContentType</param>
        /// <param name="providerID">ID of existing WebPart</param>
        /// <param name="providerField">Connection Field of existing WebPart</param>
        /// <param name="consumerField">Connection Field of the XsltListViewWebPart WebPart</param>
        internal static void AttachAndConnectListView(SPWeb web, string pageUrl, string zoneID, string listUrl,
            string providerID, string providerField, string consumerField)
        {
            var webParts = web.GetLimitedWebPartManager(pageUrl, PersonalizationScope.Shared);
            var consumer = AttachListView(web, pageUrl, zoneID, listUrl, webParts);
            //web.Update();
            //try
            //{
            //    ConnectWebParts(web, pageUrl, providerID, providerField, consumer, consumerField, webParts);
            //}
            //catch (System.Exception ex)
            //{
            //    //Log to ULS
            //    SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("My Category", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            //}
        }

    }
}
