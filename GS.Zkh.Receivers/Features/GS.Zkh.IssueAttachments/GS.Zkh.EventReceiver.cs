// <copyright file="GS.Zkh.IssueAttachments.EventReceiver.cs" company="ITB">
// Copyright ITB. All rights reserved.
// </copyright>
// <author>SPDEV\smikolaytis</author>
// <date>2015-03-26 20:56:06Z</date>
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace GS.Zkh.Receivers
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("dcea77cb-2421-428c-be0d-4a73b55deb63")]
    public class IssueAttachmentsEventReceiver : SPFeatureReceiver
    {
        private readonly string storageListTemplateId = "10156";
        private readonly string attachmentIssueFieldName = "IssueAttachmentIssueZkh";
        private readonly string meetingListName = "MeetingZkhList";
        private readonly string meetingDateFieldName = "MeetingDateZkh";
        private readonly string meetingNumberFieldName = "MeetingNumberZkh";
        private readonly string issueListName = "IssueZkhList";
        private readonly string issueMeetingFieldName = "IssueMeetingZkh";

        #region Events
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            init(properties);

            SPDiagnosticsService dsvc = SPDiagnosticsService.Local;

            if ((meetingList == null) || (questionList == null))
            {
                dsvc.WriteEvent(0,
                    new SPDiagnosticsCategory(
                        Consts.EventLogCategory,
                        TraceSeverity.Monitorable,
                        EventSeverity.Error),
                    EventSeverity.Error,
                    "�� ���������� ����������� ������� ��������� � �������� ��������",
                    new object[] { storageListTemplateId });

                return;
            }

            if (targetLib == null)
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

            try
            {
                using (new SPMonitoredScope("GS.Zkh.FeatureActivated"))
                {
                    SPListItemCollection meetingItemList = meetingList.GetItems(new SPQuery());
                    foreach (SPListItem meetingItem in meetingItemList)
                    {
                        SPFolder meetingFolder = createFolderIfNotExists(meetingItem);
                        SPListItemCollection questionItemList = getRelatedQuestionList(meetingItem);

                        foreach (SPListItem questionItem in questionItemList)
                        {
                            createDocumentSetIfNotExists(questionItem, meetingItem, meetingFolder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dsvc.WriteTrace(0,
                    new SPDiagnosticsCategory(
                        Consts.TraceLogCategory,
                        TraceSeverity.Unexpected,
                        EventSeverity.Error),
                    TraceSeverity.Unexpected,
                    "��������� ������ ��� �������� ��������� ��������� ��� �������� �������� �������� ��������: {0}",
                    new object[] { ex });
                throw;
            }
        }
        #endregion

        #region private members

        private SPList m_meetingList;
        private SPList m_questionList;
        private SPWeb m_web;
        private SPDocumentLibrary m_targetLib;

        #endregion

        public SPList meetingList
        {
            get
            {
                if (m_meetingList == null)
                {
                    m_meetingList = this.web != null ? this.web.GetList("Lists/" + meetingListName) : null;
                }

                return m_meetingList;
            }
        }

        public SPList questionList
        {
            get
            {
                if (m_questionList == null)
                {
                    m_questionList = this.web != null ? this.web.GetList("Lists/" + issueListName) : null;
                }

                return m_questionList;
            }
        }

        public SPWeb web
        {
            get
            {
                return m_web;
            }
        }

        public SPDocumentLibrary targetLib
        {
            get
            {
                if (m_targetLib == null)
                {
                    SPList lib = this.web != null ? this.web.Lists.Cast<SPList>().FirstOrDefault(
                        l => l.BaseTemplate.ToString().Equals(storageListTemplateId)) : null;

                    m_targetLib = lib as SPDocumentLibrary;
                }

                return m_targetLib;
            }
        }

        private void init(SPFeatureReceiverProperties properties)
        {
            m_web = properties.Feature.Parent as SPWeb;
            if (m_web == null)
                m_web = ((SPSite)properties.Feature.Parent).RootWeb;
        }

        private SPListItemCollection getRelatedQuestionList(SPListItem meetingItem)
        {
            SPQuery query = new SPQuery()
            {
                Query = string.Format(@"<Where>
                            <Eq>
                                <FieldRef Name='{0}' LookupId='TRUE' />
                                <Value Type='Lookup'>{1}</Value>
                            </Eq>
                        </Where>", issueMeetingFieldName, meetingItem.ID)
            };

            return questionList.GetItems(query);
        }

        private SPFolder createFolderIfNotExists(SPListItem meetingItem)
        {
            string mTitle = MeetingItem.getTitle(new MeetingTitleData()
            {
                Number = meetingItem[meetingNumberFieldName],
                Date = meetingItem[meetingDateFieldName]
            });

            SPFolder meetingFolder = SPFolderHierarchy.GetSubFolder(targetLib.RootFolder, mTitle, true);

            return meetingFolder;
        }

        private void createDocumentSetIfNotExists(SPListItem questionItem, SPListItem meetingItem, SPFolder parentFolder)
        {
            string qTitle = IssueItem.getTitle(new QuestionTitleData()
            {
                Number = questionItem
            });

            SPFolder questionFolder = SPFolderHierarchy.GetSubFolder(parentFolder, qTitle, false);

            if (questionFolder == null || !questionFolder.Exists)
            {
                Hashtable props = new Hashtable();
                props.Add("DocumentSetDescription", "������������� ��������� ����� ���������� ��� �������");
                props.Add(attachmentIssueFieldName, questionItem);

                DocumentSet.Create(parentFolder, qTitle, targetLib.ContentTypes["IssueAttachmentZkh"].Id, props, true);
            }
        }
    }
}