using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using ITB.SP.Tools;

namespace GS.GS.Land.ContentTypes.Features.GS.Land.QuestionCategoryLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e03c5ed6-3ac8-4000-bf0b-65791c013ebc")]
    public class GSLandEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName = "QuestionCategoryLand";
        private readonly string fieldGuid = "{85B72DCD-02BC-46A6-B3B6-8C77743B9A8E}";
        private readonly string fieldName = "QuestionDependentQuestionLand";

        private readonly string fieldParentFeatureId = "e2d4932e-0cc5-47ab-a254-25ac16ba1d0f";
        private readonly string fieldGroupName = "Земля.Справочники";
        private readonly string fieldDisplayName = "Родительская категория";
        private readonly string fieldDescription = "Ссылка на родительскую категорию";

        private readonly string targetShowFieldName = "QuestionCategoryNameLand";
        private readonly string targetLookupListRelativeUrl = "QuestionCategoryLandList";
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
