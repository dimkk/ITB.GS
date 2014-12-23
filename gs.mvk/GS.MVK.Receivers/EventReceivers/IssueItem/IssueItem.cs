using GS.Common.BL;
using ITB.SP.Tools;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.JSGrid;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections;
using System.Linq;
using System.Security.Permissions;

namespace GS.MVK.Receivers
{
    public class IssueItem : SPItemEventReceiver
    {
        #region Events
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemAdded(SPItemEventProperties properties)
        {
            SetIssuePStatus(properties, StatusEnum.MvkIncluded);
            EventFiringEnabled = false;
            CreateAttachment(properties);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            SetIssuePStatus(properties, StatusEnum.IgAdded);
            EventFiringEnabled = false;
            DeleteAttachment(properties);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            EventFiringEnabled = false;
            RenameAttachment(properties);
            EventFiringEnabled = true;
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            SetIssuePStatus(properties, StatusEnum.MvkConsidered);
        }
        #endregion

        #region Actions

        protected void CreateAttachment(SPItemEventProperties properties)
        {
            using (new SPMonitoredScope("GS.MVK.IssueItemAdded"))
            {
                var dsvc = SPDiagnosticsService.Local;
                var meetingItem = getMeeting(properties);

                if (meetingItem == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "������ ������� ������ {0} ��� ������ �� ������� ������ {1}",
                        new object[] { properties.ListTitle, properties.Web.GetList("Lists/" + meetingListName).Title });

                    return;
                }

                var mTitle = MeetingItem.getTitle(new MeetingTitleData()
                {
                    Number = meetingItem[meetingNumberFieldName],
                    Date = meetingItem[meetingDateFieldName]
                });

                if (properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "�� ���������� ���������� ������ �� ������� {0}. ���������� ������� ��������� ��������� ��� �������� ��������",
                        new object[] { storageListTemplateId });

                    return;
                }

                var qTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.ListItem
                });

                var props = new Hashtable
                {
                    {"DocumentSetDescription", "����� ���������� ��� ������� ��������"},
                    {attachmentIssueFieldName, properties.ListItem}
                };

                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (var site = new SPSite(properties.SiteId))
                        {
                            using (var web = site.OpenWeb(properties.RelativeWebUrl))
                            {
                                var tLib = web.Lists.Cast<SPList>().First(
                                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

                                if (tLib == null) return;
                                var folder = SPFolderHierarchy.GetSubFolder(tLib.RootFolder, mTitle, true);

                                DocumentSet.Create(folder, qTitle, tLib.ContentTypes["����� ����������"].Id, props,
                                    true);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    dsvc.WriteTrace(0,
                        new SPDiagnosticsCategory(
                            Consts.TraceLogCategory,
                            TraceSeverity.Unexpected,
                            EventSeverity.Error),
                        TraceSeverity.Unexpected,
                        "�� ������� ������� ����� ����������: {0}",
                        new object[] { ex });
                    throw;
                }
            }

        }

        protected void RenameAttachment(SPItemEventProperties properties)
        {
            try
            {
                var newTitle = getTitle(new QuestionTitleData()
                {
                    Number = properties.ListItem
                });

                var dsvc = SPDiagnosticsService.Local;
                var targetLib = properties.Web.Lists.Cast<SPList>().First(
                    l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

                if (targetLib == null)
                {
                    dsvc.WriteEvent(0,
                        new SPDiagnosticsCategory(
                            Consts.EventLogCategory,
                            TraceSeverity.Monitorable,
                            EventSeverity.Warning),
                        EventSeverity.Error,
                        "�� ���������� ���������� ������ �� ������� {0}. ���������� �������� ��������� ��������� ��� �������� ��������",
                        new object[] { storageListTemplateId });

                    return;
                }

                using (new SPMonitoredScope("GS.MVK.IssueItemUpdated"))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (var site = new SPSite(properties.SiteId))
                        using (var web = site.OpenWeb(properties.RelativeWebUrl))
                        {
                            var dsItem = doGetQuestionDocumentSet(properties.ListItemId, web);
                            if (dsItem == null)
                            {
                                dsvc.WriteEvent(0,
                                    new SPDiagnosticsCategory(
                                        Consts.EventLogCategory,
                                        TraceSeverity.Monitorable,
                                        EventSeverity.Warning),
                                    EventSeverity.Warning,
                                    "�� ������� ���������� ����� ���������� � ������ {2}, ������� �� �������������� �������� ������ {0} � Id {1}",
                                    new object[] { properties.ListTitle, properties.ListItemId, targetLib.Title });

                                return;
                            }

                            try
                            {
                                var renameTo = SPUrlUtility.CombineUrl(dsItem.Folder.ParentFolder.Url, newTitle);
                                if (dsItem.Folder.Url.Equals(renameTo, StringComparison.Ordinal))
                                    return;

                                dsItem.Folder.MoveTo(renameTo);
                            }
                            catch (Exception ex)
                            {
                                dsvc.WriteTrace(0,
                                    new SPDiagnosticsCategory(
                                        Consts.TraceLogCategory,
                                        TraceSeverity.Unexpected,
                                        EventSeverity.Error),
                                    TraceSeverity.Unexpected,
                                    "�� ������� ������������� ����� ���������� � Id {0}: {1}",
                                    new object[] { dsItem.ID, ex });
                                throw;
                            }
                        }
                    });
                }
            }
            finally
            {
                EventFiringEnabled = true;
            }
        }

        protected void DeleteAttachment(SPItemEventProperties properties)
        {
            SPDiagnosticsService dsvc = SPDiagnosticsService.Local;
            var targetLib = properties.Web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

            if (targetLib == null)
            {
                dsvc.WriteEvent(0,
                    new SPDiagnosticsCategory(
                        Consts.EventLogCategory,
                        TraceSeverity.Monitorable,
                        EventSeverity.Warning),
                    EventSeverity.Error,
                    "�� ���������� ���������� ������ �� ������� {0}. ���������� �������� ��������� ��������� ��� �������� ��������",
                    new object[] { storageListTemplateId });

                return;
            }

            using (new SPMonitoredScope("GS.MVK.IssueItemDeleting"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (var site = new SPSite(properties.SiteId))
                    using (var web = site.OpenWeb(properties.RelativeWebUrl))
                    {
                        var dsItem = doGetQuestionDocumentSet(properties.ListItemId, web);
                        if (dsItem == null)
                        {
                            dsvc.WriteEvent(0,
                                new SPDiagnosticsCategory(
                                    Consts.EventLogCategory,
                                    TraceSeverity.Monitorable,
                                    EventSeverity.Warning),
                                EventSeverity.Warning,
                                "�� ������� ���������� ����� ���������� � ������ {2}, ������� �� �������������� �������� ������ {0} � Id {1}",
                                new object[] { properties.ListTitle, properties.ListItemId, targetLib.Title });

                            return;
                        }

                        try
                        {
                            dsItem.Delete();
                        }
                        catch (Exception ex)
                        {
                            dsvc.WriteTrace(0,
                                new SPDiagnosticsCategory(
                                    Consts.TraceLogCategory,
                                    TraceSeverity.Unexpected,
                                    EventSeverity.Error),
                                TraceSeverity.Unexpected,
                                "�� ������� ������� ����� ���������� � Id {0}: {1}",
                                new object[] { dsItem.ID, ex });
                            throw;
                        }
                    }
                });
            }
        }

        protected void SetIssuePStatus(SPItemEventProperties properties, StatusEnum status)
        {
            int issuePId = 0;
            try
            {
                issuePId = properties.ListItem.GetFieldLookup(IssueMvk.IssueIssuePFieldName).LookupId;
                if (issuePId <= 0)
                    return;

                SPListItem issueP = properties.Web.GetListByUrl(IssueP.IssuePListName).GetItemById(issuePId);

                if (status == StatusEnum.MvkConsidered)
                {
                    //������������� ������ "����������� �� ���" ������ ��� ���������� �������
                    string before = Convert.ToString(properties.BeforeProperties[IssueMvk.IssueDecisionMvkFieldName]);
                    string after = Convert.ToString(properties.AfterProperties[IssueMvk.IssueDecisionMvkFieldName]);
                    if (before == after || !string.IsNullOrEmpty(before) || string.IsNullOrEmpty(after))
                        return;
                }

                IssueP.SetStatus(issueP, status);
            }
            catch (Exception ex)
            {
                Log.Unexpected(ex, "�������������� ������ ��� ��������� ������� ��������� ������� (ID = {0}) ��� ������� ��� (ID = {1})", issuePId, properties.ListItemId);
            }
        }

        private readonly string storageListTemplateId = "10056";
        private readonly string attachmentIssueFieldName = "IssueAttachmentIssueMVK";
        private readonly string issueMeetingFieldName = "IssueMeetingMVK";
        private static readonly string issueNumberFieldName = "IssueNumberTextMVK";
        private readonly string meetingListName = "MeetingMVKList";
        private readonly string meetingDateFieldName = "MeetingDateMVK";
        private readonly string meetingNumberFieldName = "MeetingNumberMVK";

        internal static string getTitle(QuestionTitleData data)
        {
            SPListItem item = data.Number as SPListItem;
            SPFieldCalculated cf = item != null ? item.Fields.GetFieldByInternalName(issueNumberFieldName) as SPFieldCalculated : null;
            object number = cf != null ? cf.GetFieldValueAsText(item[issueNumberFieldName]) : data.Number;
            string result = number != null && number.ToString() != string.Empty ? number.ToString() : "����������";
            return String.Format("������ �{0}", result);
        }

        internal SPListItem doGetQuestionDocumentSet(int Id, SPWeb web)
        {
            SPListItem returnValue = null;
            var targetLib = web.Lists.Cast<SPList>().First(
                l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) as SPDocumentLibrary;

            if (targetLib != null)
            {
                var query = new SPQuery()
                {
                    Query = string.Format(@"<Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='{0}' LookupId='TRUE' />
                                        <Value Type='Lookup'>{1}</Value>
                                    </Eq>
                                    <BeginsWith>
			                            <FieldRef Name='ContentTypeId' />
			                            <Value Type='ContentTypeId'>0x0120D520</Value>
		                            </BeginsWith>
                                </And>
                            </Where>", attachmentIssueFieldName, Id)
                };
                query.ViewAttributes = "Scope='RecursiveAll'";

                var items = targetLib.GetItems(query);
                returnValue = (items.Count != 1) ? null : items[0];
            }

            return returnValue;
        }

        public SPListItem getMeeting(SPItemEventProperties properties)
        {
            var meetingId = Helpers.getFieldLookupId(properties.ListItem, issueMeetingFieldName);

            return meetingId > 0 ? doGetMeeting(meetingId, properties.Web) : null;
        }

        private SPListItem doGetMeeting(int Id, SPWeb web)
        {
            var query = new SPQuery()
            {
                Query = @"<Where>
                            <Eq>
                                <FieldRef Name='ID' />
                                <Value Type='Integer'>" + Id + @"</Value>
                            </Eq>
                        </Where>"
            };

            var list = web.GetList("/Lists/" + meetingListName);
            var items = list.GetItems(query);

            return (items.Count != 0) ? items[0] : null;
        }

        #endregion
    }

    internal struct QuestionTitleData
    {
        public object Number;
    }
}

