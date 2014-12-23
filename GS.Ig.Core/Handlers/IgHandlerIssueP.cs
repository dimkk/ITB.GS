using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Common.BL;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using SAMRT.Ig.Interface;
using IIgHandler = GS.Ig.Core.Interfaces.IIgHandler;

namespace GS.Ig.Core.Handlers
{
    public class IgHandlerIssueP : IIgHandler
    {
        public bool IsItemOwner(SPListItem target)
        {
            return target.ParentList.RootFolder.ServerRelativeUrl.EndsWith(IssueP.IssuePListName, StringComparison.OrdinalIgnoreCase);
        }

        public void Process(SPListItem issueP, StatusEnum newStatus)
        {
            //Отправляем сущности в САМРТ только при определенных статусах
            if (newStatus == StatusEnum.IgAdded ||
                newStatus == StatusEnum.MvkIncluded ||
                newStatus == StatusEnum.MvkConsidered ||
                newStatus == StatusEnum.GsIncluded ||
                newStatus == StatusEnum.GsConsidered)
            {
                IgSenderManager.SendEntity(issueP, issueP.GetFieldValue<string>(IssueP.IssuePSourceTypeFieldName).EnumParse<IgEntityType>());
            }
        }
    }
}
