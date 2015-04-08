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
                properties.AfterProperties["Title"] = "Просмотреть отчет";
            }
            catch (Exception e)
            {
                Log.Unexpected(e, "При установке названия отчета по поручению произошло неожиданное исключение");
            }
        }

        private void SetInfo(SPItemEventProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                try
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.Web.ID))
                    {
                        int assignmentId;
                        if (int.TryParse(Convert.ToString(properties.AfterProperties["AssignmentLink"]), out assignmentId) &&
                            assignmentId > 0)
                        {
                            SPListItem assignment = web.GetListByUrl("AssignmentList").GetItemById(assignmentId);
                            properties.AfterProperties["AssignmentReportRequestText"] = assignment["Инфо"];
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Unexpected(e,
                        "При установке поля с информацией отчета по поручению (ID = {0}) произошло неожиданное исключение",
                        properties.AfterProperties["ID"]);
                }
            });
        }

        public void UpdateAssignment(SPItemEventProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                int assignmentId = 0;
                try
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.Web.ID))
                    {
                        assignmentId = properties.ListItem.GetFieldLookup("AssignmentLink").LookupId;
                        if (assignmentId > 0)
                        {
                            SPListItem assignment = web.GetListByUrl("AssignmentList").GetItemById(assignmentId);
                            assignment["Последний отчет"] = new SPFieldLookupValue(properties.ListItemId,
                                properties.ListItem.Title);

                            string status = null;
                            string controlStatus = null;

                            var decision = properties.ListItem.GetFieldValue<string>("AssignmentReportResolutionDecision");
                            if (decision == "Снять с контроля")
                            {
                                status = "Исполнено";
                                controlStatus = "Снято с контроля";
                                var newFactDate =
                                    properties.ListItem.GetFieldValue<DateTime?>("AssignmentReportFactAnswerDate");
                                if (newFactDate.HasValue)
                                    assignment["AssignmentFactDate"] = newFactDate.Value;
                            }
                            else if (decision == "Перенести срок")
                            {
                                status = "На исполнении";
                                controlStatus = "На контроле";
                                var newDate =
                                    properties.ListItem.GetFieldValue<DateTime?>("AssignmentReportResolutionNewDate");
                                if (newDate.HasValue && newDate.Value > new DateTime(2010, 1, 1))
                                    assignment["AssignmentPlanDate"] = newDate.Value;
                            }

                            if (!string.IsNullOrEmpty(status))
                                assignment["AssignmentStatus"] = status;
                            if (!string.IsNullOrEmpty(controlStatus))
                                assignment["AssignmentInspectState"] = controlStatus;

                            assignment.SystemUpdate();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Unexpected(e,
                        "При обновлении полей поручения (ID = {0}) в обработчике событий отчета по поручению (ID = {1}) произошло неожиданное исключение",
                        assignmentId, properties.ListItemId);
                }
            });
        }
        #endregion
    }
}

