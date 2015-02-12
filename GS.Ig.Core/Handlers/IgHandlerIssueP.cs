using GS.Common.BL;
using GS.Ig.Interfaces;
using ITB.Ig.Interface;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;

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
