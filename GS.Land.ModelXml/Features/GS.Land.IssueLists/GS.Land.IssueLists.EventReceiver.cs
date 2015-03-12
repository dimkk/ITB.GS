using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Runtime.InteropServices;

namespace GS.Land.ModelXml.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("fac94984-e74b-4b2b-8b0f-170f90f9fe5c")]
    public class IssueListEventReceiver : SPFeatureReceiver
    {
        private readonly string fieldContentTypeName = "IssueLand";
        private readonly string fieldName = "IssueNumberTextLand";

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
