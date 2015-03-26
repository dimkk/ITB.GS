using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace GS.Zkh.ContentTypes.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("280375a3-92bd-4159-8924-376202493cd3")]
    public class MeetingZkhListEventReceiver : SPFeatureReceiver
    {
        private readonly string fieldContentTypeName = "MeetingZkh";
        private readonly string fieldName = "MeetingDateNumberZkh";

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
