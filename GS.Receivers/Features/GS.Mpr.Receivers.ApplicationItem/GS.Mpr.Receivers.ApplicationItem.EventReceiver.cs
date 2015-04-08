using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace GS.Mpr.Receivers
{
    [Guid("227afee7-fdf9-43bc-9ad1-37974069f825")]
    public class ApplicationItemFeatureReceiver : SPFeatureReceiver
    {
        #region Events
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            CreateDocumentSets(properties.Feature.Parent as SPWeb);
        }
        #endregion

        #region Actions
        private void CreateDocumentSets(SPWeb web)
        {
            try
            {
                if (web == null)
                    throw new ArgumentNullException("web");

                SPList targetList = web.GetListByUrl(ConclusionSetItem.ApplicationMprListName);
                targetList.Foreach(ApplicationItem.CreateDocumentSet);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "Ќеожиданное исключение при создании наборов документов дл€ списка {0}", ConclusionSetItem.ApplicationMprListName);
            }
        }
        #endregion
    }
}
