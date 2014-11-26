using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System;
using System.Security.Permissions;

namespace GS.Receivers
{
    public class AssignmentReportItem : SPItemEventReceiver
    {
        #region Events
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdding(SPItemEventProperties properties)
        {
            SetTitle(properties);
            SetInfo(properties);
            base.ItemAdding(properties);
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            UpdateAssignment(properties);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            SetInfo(properties);
            base.ItemUpdating(properties);
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            UpdateAssignment(properties);
            EventFiringEnabled = true;
        }
        #endregion

        #region Actions
        private void SetTitle(SPItemEventProperties properties)
        {
            try
            {
                properties.AfterProperties["Title"] = "����������� �����";
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� ��������� �������� ������ �� ��������� ��������� ����������� ����������");
            }
        }

        private void SetInfo(SPItemEventProperties properties)
        {
            try
            {
                int assignmentId;
                if (int.TryParse(Convert.ToString(properties.AfterProperties["AssignmentLink"]), out assignmentId) && assignmentId > 0)
                {
                    SPListItem assignment = properties.Web.GetListByUrl("AssignmentList").GetItemById(assignmentId);
                    properties.AfterProperties["AssignmentReportRequestText"] = assignment["����"];
                }
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� ��������� ���� � ����������� ������ �� ��������� ��������� ����������� ����������");
            }
        }

        public void UpdateAssignment(SPItemEventProperties properties)
        {
            try
            {
                int assignmentId = properties.ListItem.GetFieldLookup("AssignmentLink").LookupId;
                if (assignmentId > 0)
                {
                    SPListItem assignment = properties.Web.GetListByUrl("AssignmentList").GetItemById(assignmentId);
                    assignment["��������� �����"] = new SPFieldLookupValue(properties.ListItemId, properties.ListItem.Title);

                    string status = null;
                    string controlStatus = null;

                    var decision = properties.ListItem.GetFieldValue<string>("AssignmentReportResolutionDecision");
                    if (decision == "����� � ��������")
                    {
                        status = "���������";
                        controlStatus = "����� � ��������";
                        var newFactDate = properties.ListItem.GetFieldValue<DateTime?>("AssignmentReportFactAnswerDate");
                        if (newFactDate.HasValue)
                            assignment["AssignmentFactDate"] = newFactDate.Value;
                    }
                    else if (decision == "��������� ����")
                    {
                        status = "�� ����������";
                        controlStatus = "�� ��������";
                        var newDate = properties.ListItem.GetFieldValue<DateTime?>("AssignmentReportResolutionNewDate");
                        if (newDate.HasValue && newDate.Value > new DateTime(2010, 1, 1))
                            assignment["AssignmentPlanDate"] = newDate.Value;
                    }

                    if (!string.IsNullOrEmpty(status))
                        assignment["AssignmentStatus"] = status;
                    if (!string.IsNullOrEmpty(controlStatus))
                        assignment["AssignmentInspectState"] = controlStatus;

                    assignment.Update();
                }
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "��� ���������� ����� ��������� ��������� ����������� ����������", properties.ListItemId);
            }
        }
        #endregion
    }
}

