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

namespace GS.Zkh.WebParts.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [Guid("f2575108-f5fa-4d1c-b4bd-7ffb2e0c809b")]
    public class WebPartsEventReceiver : SPFeatureReceiver
    {
        SPWeb web = null;

        private void init(SPFeatureReceiverProperties properties)
        {
            web = properties.Feature.Parent as SPWeb;
            if (web == null)
                web = ((SPSite)properties.Feature.Parent).RootWeb;
            web.AllowUnsafeUpdates = true;
        }

        private void AttachWebParts()
        {
            //ReportZkh
            WPMaster.AttachAndConnectListView(web, "Lists/ReportZkhList/DispForm2.aspx", "wzAttaches",
                "Lists/ReportAttachmentZkhList", "g_00042e20_1d57_49a0_9a17_2e7c6797b968",
                "ID", "ReportAttachmentReportZkh");

            WPMaster.AttachAndConnectListView(web, "Lists/ReportZkhList/EditForm2.aspx", "wzAttaches",
                "Lists/ReportAttachmentZkhList", "g_65d1e125_7eda_4716_a39b_def5d6b2c260",
                "ID", "ReportAttachmentReportZkh");

            //AssignmentZkh
            WPMaster.AttachAndConnectListView(web, "Lists/AssignmentZkhList/DispForm2.aspx", "wzReports",
                "Lists/ReportZkhList", "g_0fb6b814_b3a7_418b_bbbd_6a524ef6788f",
                "ID", "ReportAssignmentZkh");

            WPMaster.AttachAndConnectListView(web, "Lists/AssignmentZkhList/EditForm2.aspx", "wzReports",
                "Lists/ReportZkhList", "g_54851d06_61af_4ead_bd33_748770279fa0",
                "ID", "ReportAssignmentZkh");

            //IssueZkh
            WPMaster.AttachAndConnectListView(web, "Lists/IssueZkhList/DispForm2.aspx", "wzAssignments",
                "Lists/AssignmentZkhList", "g_1b5e5642_5c24_4803_a46c_94a6922ae218",
                "ID", "AssignmentIssueZkh");

            WPMaster.AttachAndConnectListView(web, "Lists/IssueZkhList/EditForm2.aspx", "wzAssignments",
                "Lists/AssignmentZkhList", "g_fd604c01_8495_4d45_8d16_2b63897cec4e",
                "ID", "AssignmentIssueZkh");

            WPMaster.AttachAndConnectListView(web, "Lists/IssueZkhList/DispForm2.aspx", "wzAttaches",
                "Lists/IssueAttachmentZkhList", "g_1b5e5642_5c24_4803_a46c_94a6922ae218",
                "ID", "IssueAttachmentIssueZkh");

            WPMaster.AttachAndConnectListView(web, "Lists/IssueZkhList/EditForm2.aspx", "wzAttaches",
                "Lists/IssueAttachmentZkhList", "g_fd604c01_8495_4d45_8d16_2b63897cec4e",
                "ID", "IssueAttachmentIssueZkh");

            //ArgumentExceptionFix
            //WPMaster.SetViewStyle(web, "Lists/AssignmentZkhList", 17);
            //WPMaster.SetViewStyle(web, "Lists/IssueZkhList", 17);

            web.Update();
        }
        private void DetachWebParts()
        {
            WPMaster.DetachListViews(web, "Lists/ReportZkhList/DispForm2.aspx", "wzAttaches");
            WPMaster.DetachListViews(web, "Lists/ReportZkhList/EditForm2.aspx", "wzAttaches");

            WPMaster.DetachListViews(web, "Lists/AssignmentZkhList/DispForm2.aspx", "wzReports");
            WPMaster.DetachListViews(web, "Lists/AssignmentZkhList/EditForm2.aspx", "wzReports");

            WPMaster.DetachListViews(web, "Lists/IssueZkhList/DispForm2.aspx", "wzAssignments");
            WPMaster.DetachListViews(web, "Lists/IssueZkhList/EditForm2.aspx", "wzAssignments");
            WPMaster.DetachListViews(web, "Lists/IssueZkhList/DispForm2.aspx", "wzAttaches");
            WPMaster.DetachListViews(web, "Lists/IssueZkhList/EditForm2.aspx", "wzAttaches");

            web.Update();
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            init(properties);
            DetachWebParts();
            try
            {
                AttachWebParts();
            }
            catch
            {
                DetachWebParts();
                throw;
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            init(properties);
            DetachWebParts();
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            init(properties);
            DetachWebParts();
        }

        public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        {
            init(properties);
            DetachWebParts();
        }

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}
    }
}
