//Sergey Mikolaytis
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System.Globalization;
using System.Text;

namespace GS.Land.WebParts.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("c1f0d232-fc9f-428c-9e92-34b503292bb8")]
    public class WebPartsEventReceiver : SPFeatureReceiver
    {
        /// <summary>
        /// Attach WebParts
        /// </summary>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (var site = new SPSite("http://sp2013dev:81/"))//SPContext.Current.Site.Url
            using (var web = site.RootWeb)
            {
                web.AllowUnsafeUpdates = true;

                //WPMaster.DetachListViews(web, "Lists/ReportLandList/DispForm2.aspx", "wzAttaches");
                //WPMaster.DetachListViews(web, "Lists/ReportLandList/EditForm2.aspx", "wzAttaches");

                //WPMaster.DetachListViews(web, "Lists/AssignmentLandList/DispForm2.aspx", "wzReports");
                //WPMaster.DetachListViews(web, "Lists/AssignmentLandList/EditForm2.aspx", "wzReports");

                //WPMaster.DetachListViews(web, "Lists/IssueLandList/DispForm2.aspx", "wzAssignments");
                //WPMaster.DetachListViews(web, "Lists/IssueLandList/EditForm2.aspx", "wzAssignments");
                //WPMaster.DetachListViews(web, "Lists/IssueLandList/DispForm2.aspx", "wzAttaches");
                //WPMaster.DetachListViews(web, "Lists/IssueLandList/EditForm2.aspx", "wzAttaches");

                //web.Update();

                //WPMaster.AttachAndConnectListView(web, "Lists/ReportLandList/DispForm2.aspx", "wzAttaches",
                //    "Lists/ReportAttachmentLandList", "g_00042e20_1d57_49a0_9a17_2e7c6797b968",
                //    "ID", "ReportAttachmentReportLand");
                //WPMaster.AttachAndConnectListView(web, "Lists/ReportLandList/EditForm2.aspx", "wzAttaches",
                //    "Lists/ReportAttachmentLandList", "g_65d1e125_7eda_4716_a39b_def5d6b2c260",
                //    "ID", "ReportAttachmentReportLand");

                var wp = WPMaster.AttachListView(web, "Lists/ReportLandList/DispForm2.aspx", "wzAttaches",
                    "Lists/ReportAttachmentLandList");
                wp = WPMaster.AttachListView(web, "Lists/ReportLandList/EditForm2.aspx", "wzAttaches",
                   "Lists/ReportAttachmentLandList");

                wp = WPMaster.AttachListView(web, "Lists/AssignmentLandList/DispForm2.aspx", "wzReports",
                    "Lists/ReportLandList");
                wp = WPMaster.AttachListView(web, "Lists/AssignmentLandList/EditForm2.aspx", "wzReports",
                    "Lists/ReportLandList");

                wp = WPMaster.AttachListView(web, "Lists/IssueLandList/DispForm2.aspx", "wzAssignments",
                    "Lists/AssignmentLandList");
                wp = WPMaster.AttachListView(web, "Lists/IssueLandList/EditForm2.aspx", "wzAssignments",
                    "Lists/AssignmentLandList");

                wp = WPMaster.AttachListView(web, "Lists/IssueLandList/DispForm2.aspx", "wzAttaches",
                    "Lists/IssueAttachmentLandList");
                wp = WPMaster.AttachListView(web, "Lists/IssueLandList/EditForm2.aspx", "wzAttaches",
                    "Lists/IssueAttachmentLandList");

                web.Update();
            }
        }

        /// <summary>
        /// Detach WebParts
        /// </summary>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            using (var site = new SPSite(SPContext.Current.Site.Url))
            using (var web = site.RootWeb)
            {
                WPMaster.DetachListViews(web, "Lists/ReportLandList/DispForm2.aspx", "wzAttaches");
                WPMaster.DetachListViews(web, "Lists/ReportLandList/EditForm2.aspx", "wzAttaches");

                WPMaster.DetachListViews(web, "Lists/AssignmentLandList/DispForm2.aspx", "wzReports");
                WPMaster.DetachListViews(web, "Lists/AssignmentLandList/EditForm2.aspx", "wzReports");

                WPMaster.DetachListViews(web, "Lists/IssueLandList/DispForm2.aspx", "wzAssignments");
                WPMaster.DetachListViews(web, "Lists/IssueLandList/EditForm2.aspx", "wzAssignments");
                WPMaster.DetachListViews(web, "Lists/IssueLandList/DispForm2.aspx", "wzAttaches");
                WPMaster.DetachListViews(web, "Lists/IssueLandList/EditForm2.aspx", "wzAttaches");

                web.Update();
            }

        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
