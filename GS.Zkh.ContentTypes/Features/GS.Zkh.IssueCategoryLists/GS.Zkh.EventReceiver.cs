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

    [Guid("5698bec3-6dda-4974-8b53-e226b374ea47")]
    public class IssueCategoryListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName = "IssueCategoryZkh";
        private readonly string fieldGuid = "{C3440AC5-B2A3-4997-AB71-0DF125540096}";
        private readonly string fieldName = "IssueDependentIssueZkh";

        private readonly string fieldParentFeatureId = "7bf01554-94ed-4030-ab3d-7c8a4aeecec0";
        private readonly string fieldGroupName = "ЖКХ.Справочники";
        private readonly string fieldDisplayName = "Родительская категория";
        private readonly string fieldDescription = "Ссылка на родительскую категорию";

        private readonly string targetShowFieldName = "Title";
        private readonly string targetLookupListRelativeUrl = "IssueCategoryZkhList";
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
