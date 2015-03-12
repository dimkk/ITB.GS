using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using ITB.SP.Tools;

namespace GS.Land.ModelXml.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e03c5ed6-3ac8-4000-bf0b-65791c013ebc")]
    public class IssueCategoryListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName = "IssueCategoryLand";
        private readonly string fieldGuid = "{85B72DCD-02BC-46A6-B3B6-8C77743B9A8E}";
        private readonly string fieldName = "IssueDependentIssueLand";

        private readonly string fieldParentFeatureId = "e2d4932e-0cc5-47ab-a254-25ac16ba1d0f";
        private readonly string fieldGroupName = "�����.�����������";
        private readonly string fieldDisplayName = "������������ ���������";
        private readonly string fieldDescription = "������ �� ������������ ���������";

        private readonly string targetShowFieldName = "Title";
        private readonly string targetLookupListRelativeUrl = "IssueCategoryLandList";
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
