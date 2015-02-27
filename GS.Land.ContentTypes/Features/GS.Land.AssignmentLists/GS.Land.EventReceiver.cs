using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using ITB.SP.Tools;

namespace GS.GS.Land.ContentTypes.Features.GS.Land.AssignmentLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("82577d19-6d85-4fde-95a1-dccf54c1bc9c")]
    public class GSLandEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName = "AssignmentLand";
        private readonly string fieldGuid = "{9BD247E2-7E15-4D4F-BE0E-1427F9AA869A}";
        private readonly string fieldName = "AssignmentDependentAssignmentLand";

        private readonly string fieldParentFeatureId = "c312542e-547b-4557-a5fa-99b5c657ce24";
        private readonly string fieldGroupName = "Земля.Поручения";
        private readonly string fieldDisplayName = "Зависимое поручение";
        private readonly string fieldDescription = "Ссылка на зависимое поручение Земля";

        private readonly string targetShowFieldName = "AssignmentNumberLand";
        private readonly string targetLookupListRelativeUrl = "AssignmentLandList";
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
