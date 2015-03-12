using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace GS.Land.ContentTypes.Features.GS.Land.MeetingLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("51eb5a6f-8659-4294-81f7-73d6819f11aa")]
    public class MeetingLandListEventReceiver : SPFeatureReceiver
    {
        private readonly string fieldContentTypeName = "MeetingLand";
        private readonly string fieldName = "MeetingDateNumberLand";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Добавлять вычисляемое поле в тип содержимого можно только после создания экземляра списка
            using (var site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RootWeb.AddExistField(fieldContentTypeName, site.RootWeb.Fields.GetFieldByInternalName(fieldName));
            }
        }
    }
}
