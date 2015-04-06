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

    [Guid("a613f4a5-d93f-44e0-8de0-a5787429ec51")]
    public class IssueZkhListEventReceiver : SPFeatureReceiver
    {
        private readonly string fieldContentTypeName = "IssueZkh";
        private readonly string fieldName = "IssueNumberTextZkh";

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
