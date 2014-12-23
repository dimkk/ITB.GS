using GS.Common.BL;
using GS.Ig;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System;
using System.Security.Permissions;

namespace GS.Receivers
{
    public class IssuePItem : SPItemEventReceiver
    {
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                StatusEnum? status = properties.ListItem.GetFieldValue<string>(IssueP.IssuePStatusFieldName).EnumTryParse<StatusEnum>();
                if (status.HasValue)
                    IgHandlerFactory.TryProcess(properties.ListItem, status.Value);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� ���������� ��������� ������� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }

            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;

            try
            {
                int statusId1 = 0;
                int statusId2 = 0;
                int.TryParse(Convert.ToString(properties.BeforeProperties[IssueP.IssuePStatusFieldName]), out statusId1);
                int.TryParse(Convert.ToString(properties.AfterProperties[IssueP.IssuePStatusFieldName]), out statusId2);
                StatusEnum? before = statusId1 > 0 ? Status.GetById(properties.Web, statusId1) : (StatusEnum?)null;
                StatusEnum? after = statusId2 > 0 ? Status.GetById(properties.Web, statusId2) : (StatusEnum?)null;
                //���������� �� ���������� ������ ��� ��������� �������
                if (after.HasValue && (!before.HasValue || before.Value != after.Value))
                    IgHandlerFactory.TryProcess(properties.ListItem, after.Value);
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� �������� ���������� ��������� ������� (ID = {0}) ��������� ����������� ����������", properties.ListItemId);
            }

            EventFiringEnabled = true;
        }
    }
}

