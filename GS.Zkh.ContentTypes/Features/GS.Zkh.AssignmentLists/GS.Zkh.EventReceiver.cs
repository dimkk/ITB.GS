using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using ITB.SP.Tools;

namespace GS.Zkh.ContentTypes.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("60e98eda-a2bd-4fe0-9749-5b811dacf7a2")]
    public class AssignmentListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName = "AssignmentZkh";
        private readonly string fieldGuid = "{15F59E2C-49B8-442A-B5EA-F8B4ED928F83}";
        private readonly string fieldName = "AssignmentDependentAssignmentZkh";

        private readonly string fieldParentFeatureId = "faaa7738-7496-468c-b8e6-06610066b8b1";
        private readonly string fieldGroupName = "ЖКХ.Поручения";
        private readonly string fieldDisplayName = "Зависимое поручение";
        private readonly string fieldDescription = "Ссылка на зависимое поручение ЖКХ";

        private readonly string targetShowFieldName = "AssignmentNumberZkh";
        private readonly string targetLookupListRelativeUrl = "AssignmentZkhList";
        #endregion

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RootWeb.AddLookupField(fieldParentFeatureId, fieldContentTypeName, fieldGuid, fieldName, fieldGroupName, fieldDisplayName, fieldDescription, targetShowFieldName, targetLookupListRelativeUrl);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RootWeb.DeleteField(fieldName);
            }
        }
    }
}
