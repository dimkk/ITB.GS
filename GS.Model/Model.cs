using Microsoft.SharePoint.Client;
using SPMeta2.CSOM.Behaviours;
using SPMeta2.CSOM.DefaultSyntax;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.CSOM.Services;
using SPMeta2.Definitions;
using SPMeta2.Models;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GS.Model.Definitions;
using GS.Model.Definitions.Fields;

namespace GS.Model
{
    public class Model
    {
        protected ClientContext Context;
        protected CSOMProvisionService Service;

        protected IList<List> AllLists;

        public Model(string webUrl, string login, string password, string domain)
        {
            Context = new ClientContext(webUrl);
            Context.ExecutingWebRequest += (sender, e) => e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            Context.Credentials = new NetworkCredential(login, password, domain);

            Service = new CSOMProvisionService();

            IEnumerable<List> lists = Context.LoadQuery(Context.Web.Lists.Include(c => c.RootFolder.Name, c => c.Id));
            Context.Load(Context.Web);
            Context.ExecuteQuery();
            AllLists = lists.ToList();
        }

        public void Deploy()
        {
            SiteModelHost siteModelHost = SiteModelHost.FromClientContext(Context);
            WebModelHost webModelHost = WebModelHost.FromClientContext(Context);

            ModelNode siteModel = SPMeta2Model.NewSiteModel(new SiteDefinition() { RequireSelfProcessing = false });
            
            //Добавляем типы поля и типы содержимого
            //AddIgHistoryContentTypes(siteModel);
            //AddIgMessageContentTypes(siteModel);
            //AddMunicipalityContentTypes(siteModel);
            //AddConfigurationContentTypes(siteModel);
            //AddStatusContentTypes(siteModel);
            //AddBuilderContentTypes(siteModel);
            AddIssueGsContentTypes1(siteModel);

            Service.DeployModel(siteModelHost, siteModel);

            ModelNode webModel = SPMeta2Model.NewWebModel(new WebDefinition() { RequireSelfProcessing = false });

            //Добавляем списки
            //AddIgHistoryList(webModel);
            //AddIgMessageList(webModel);
            //AddMunicipalityList(webModel);
            //AddConfigurationList(webModel);
            //AddStatusList(webModel);
            //AddBuilderList(webModel);
            AddIssueGsList(webModel);

            Service.DeployModel(webModelHost, webModel);

            //CleanListContentTypes(ListModel.IgHistory.Url, ContentTypeModel.IgHistory.Name);
            //CleanListContentTypes(ListModel.IgMessage.Url, ContentTypeModel.IgMessage.Name);
            //CleanListContentTypes(ListModel.Municipality.Url, ContentTypeModel.Municipality.Name);
            //CleanListContentTypes(ListModel.Configuration.Url, ContentTypeModel.Configuration.Name);
            //CleanListContentTypes(ListModel.Status.Url, ContentTypeModel.Status.Name);
            //CleanListContentTypes(ListModel.Builder.Url, ContentTypeModel.Builder.Name);
            //CleanListContentTypes(ListModel.IssueGs.Url, ContentTypeModel.IssueGs.Name);
        }

        #region ContentTypes
        protected void AddIgHistoryContentTypes(ModelNode modelNode)
        {
            List issuePlanList = AllLists.Single(s => s.RootFolder.Name == "IssuePList");

            modelNode
                .WithFields(fields => fields
                    .AddField(IgHistoryModel.IgHistoryIssuePlan, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issuePlanList.Id, "Title")))
                    .AddField(IgHistoryModel.IgHistoryDictionary)
                    .AddField(IgHistoryModel.IgHistoryDirection)
                    .AddField(IgHistoryModel.IgHistorySenderSystem)
                    .AddField(IgHistoryModel.IgHistoryReceiverSystem)
                    .AddField(IgHistoryModel.IgHistoryStatus)
                    .AddField(IgHistoryModel.IgHistoryError)
                    .AddField(IgHistoryModel.IgHistorySendTryCount)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IgHistory, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IgHistoryModel.IgHistoryIssuePlan,
                            IgHistoryModel.IgHistoryDictionary,
                            IgHistoryModel.IgHistoryDirection,
                            IgHistoryModel.IgHistorySenderSystem,
                            IgHistoryModel.IgHistoryReceiverSystem,
                            IgHistoryModel.IgHistoryStatus,
                            IgHistoryModel.IgHistoryError,
                            IgHistoryModel.IgHistorySendTryCount)
                    ));

        }

        protected void AddIgMessageContentTypes(ModelNode modelNode)
        {
            List igHistoryList = AllLists.Single(s => s.RootFolder.Name == "IgHistory");
            List igMessageList = AllLists.Single(s => s.RootFolder.Name == "IgMessage");

            modelNode
                .WithFields(fields => fields
                    .AddField(IgMessageModel.IgMessageIgHistory, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, igHistoryList.Id, "Title")))
                    .AddField(IgMessageModel.IgMessageParentIgMessage, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, igMessageList.Id, "Title")))
                    .AddField(IgMessageModel.IgMessageHandlerVersion)
                    .AddField(IgMessageModel.IgMessageDirection)
                    .AddField(IgMessageModel.IgMessageSenderSystem)
                    .AddField(IgMessageModel.IgMessageReceiverSystem)
                    .AddField(IgMessageModel.IgMessageDataVersion)
                    .AddField(IgMessageModel.IgMessageType)
                    .AddField(IgMessageModel.IgMessageContent)
                    .AddField(IgMessageModel.IgMessageIsSuccess)
                    .AddField(IgMessageModel.IgMessageError)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IgMessage, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IgMessageModel.IgMessageIgHistory,
                            IgMessageModel.IgMessageParentIgMessage,
                            IgMessageModel.IgMessageHandlerVersion,
                            IgMessageModel.IgMessageDirection,
                            IgMessageModel.IgMessageSenderSystem,
                            IgMessageModel.IgMessageReceiverSystem,
                            IgMessageModel.IgMessageDataVersion,
                            IgMessageModel.IgMessageType,
                            IgMessageModel.IgMessageContent,
                            IgMessageModel.IgMessageIsSuccess,
                            IgMessageModel.IgMessageError)
                    ));

        }

        protected void AddMunicipalityContentTypes(ModelNode modelNode)
        {
            List igMunicipalityList = AllLists.Single(s => s.RootFolder.Name == "Municipality");

            modelNode
                .WithFields(fields => fields
                    .AddField(MunicipalityModel.MunicipalityParentMunicipality, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, igMunicipalityList.Id, "Title")))
                    .AddField(MunicipalityModel.MunicipalityType)
                    .AddField(MunicipalityModel.MunicipalityOkato)
                    .AddField(MunicipalityModel.MunicipalityExtId)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.Municipality, contentType => contentType
                        .AddContentTypeFieldLinks(
                            MunicipalityModel.MunicipalityParentMunicipality,
                            MunicipalityModel.MunicipalityType,
                            MunicipalityModel.MunicipalityOkato,
                            MunicipalityModel.MunicipalityExtId)
                    ));

        }

        protected void AddConfigurationContentTypes(ModelNode modelNode)
        {
            List configurationList = AllLists.Single(s => s.RootFolder.Name == "Configuration");

            modelNode
                .WithFields(fields => fields
                    .AddField(ConfigurationModel.ConfigurationParent, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, configurationList.Id, "Title")))
                    .AddField(ConfigurationModel.ConfigurationGroup)
                    .AddField(ConfigurationModel.ConfigurationKey)
                    .AddField(ConfigurationModel.ConfigurationValue)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.Configuration, contentType => contentType
                        .AddContentTypeFieldLinks(
                            ConfigurationModel.ConfigurationParent,
                            ConfigurationModel.ConfigurationGroup,
                            ConfigurationModel.ConfigurationKey,
                            ConfigurationModel.ConfigurationValue)
                    ));
        }

        protected void AddStatusContentTypes(ModelNode modelNode)
        {
            modelNode
                .WithFields(fields => fields
                    .AddField(StatusModel.StatusKey)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.Status, contentType => contentType
                        .AddContentTypeFieldLinks(
                            StatusModel.StatusKey)
                    ));

        }

        protected void AddBuilderContentTypes(ModelNode modelNode)
        {
            List builderList = AllLists.Single(s => s.RootFolder.Name == ListModel.Builder.Url);

            modelNode
                .WithFields(fields => fields
                    .AddField(BuilderModel.BuilderParent, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, builderList.Id, "Title")))
                    .AddField(BuilderModel.BuilderInn)
                    .AddField(BuilderModel.BuilderForm)
                    .AddField(BuilderModel.BuilderLegalAddress)
                    .AddField(BuilderModel.BuilderFactAddress)
                    .AddField(BuilderModel.BuilderExtId)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.Builder, contentType => contentType
                        .AddContentTypeFieldLinks(
                            BuilderModel.BuilderParent,
                            BuilderModel.BuilderInn,
                            BuilderModel.BuilderForm,
                            BuilderModel.BuilderLegalAddress,
                            BuilderModel.BuilderFactAddress,
                            BuilderModel.BuilderExtId)
                    ));
        }

        protected void AddIssueGsContentTypes1(ModelNode modelNode)
        {
            List issueGsList = AllLists.Single(s => s.RootFolder.Name == "AgendaQuestionList");
            List municipalityList = AllLists.Single(s => s.RootFolder.Name == ListModel.Municipality.Url);
            List participantList = AllLists.Single(s => s.RootFolder.Name == "ParticipantBookList");
            List meetingList = AllLists.Single(s => s.RootFolder.Name == "MeetingList");
            List organizationList = AllLists.Single(s => s.RootFolder.Name == "OrganizationBookList");
            List issuePList = AllLists.Single(s => s.RootFolder.Name == "IssuePList");
            List categoryList = AllLists.Single(s => s.RootFolder.Name == "AgendaQuestionCategoryBookList");
            List objectTypeList = AllLists.Single(s => s.RootFolder.Name == "List2");
            List decisionTypeList = AllLists.Single(s => s.RootFolder.Name == "DecisionTypeBookList");

            modelNode
                .WithFields(fields => fields
                    .AddField(IssueGsModel1.IssueMunicipalityGs, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, municipalityList.Id, IssueGsModel1.IssueMunicipalityGs.ShowField)))
                    .AddField(IssueGsModel1.IssueSettlementGs, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, municipalityList.Id, IssueGsModel1.IssueSettlementGs.ShowField)))
                    .AddField(IssueGsModel1.AgendaQuestionReporter, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, participantList.Id, IssueGsModel1.AgendaQuestionReporter.ShowField)))
                    .AddField(IssueGsModel1.MeetingLink, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id, IssueGsModel1.MeetingLink.ShowField)))
                    .AddField(IssueGsModel1.AgendaQuestionDeclarant, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, organizationList.Id, IssueGsModel1.AgendaQuestionDeclarant.ShowField)))
                    .AddField(IssueGsModel1.QuestionCategoryLink, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, categoryList.Id, IssueGsModel1.QuestionCategoryLink.ShowField)))
                    .AddField(IssueGsModel1.IssueGsIssueP, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issuePList.Id, IssueGsModel1.IssueGsIssueP.ShowField)))
                    .AddField(IssueGsModel1.AgendaLinkedQuestionLink, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueGsList.Id, IssueGsModel1.AgendaLinkedQuestionLink.ShowField)))
                    .AddField(IssueGsModel1.AgendaQuestionCoreporter, field =>  //MULTI
                        field.OnCreated((fieldDef, spField) =>
                        {
                            spField.MakeLookupConnectionToList(Context.Web.Id, participantList.Id, IssueGsModel1.AgendaQuestionCoreporter.ShowField);
                            ((FieldLookup)spField).AllowMultipleValues = true;
                        }))
                    .AddField(IssueGsModel1.AgendaQuestionObjectType, field =>  //MULTI
                        field.OnCreated((fieldDef, spField) =>
                        {
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectTypeList.Id, IssueGsModel1.AgendaQuestionObjectType.ShowField);
                            ((FieldLookup)spField).AllowMultipleValues = true;
                        }))
                    .AddField(IssueGsModel1.AgendaQuestionDecisionType, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, decisionTypeList.Id, IssueGsModel1.AgendaQuestionDecisionType.ShowField)))
                    //.AddField(IssueGsModel1.MeetingDate, field =>
                    //    field.OnCreated((fieldDef, spField) =>
                    //        spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id, IssueGsModel1.MeetingDate.ShowField)))
                    //.AddField(IssueGsModel1.MeetingDateText, field =>
                    //    field.OnCreated((fieldDef, spField) =>
                    //        spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id, IssueGsModel1.MeetingDateText.ShowField)))
                    //.AddField(IssueGsModel1.AgendaQuestionDeclarantId, field =>
                    //    field.OnCreated((fieldDef, spField) =>
                    //        spField.MakeLookupConnectionToList(Context.Web.Id, organizationList.Id, IssueGsModel1.AgendaQuestionDeclarantId.ShowField)))
                    //.AddField(IssueGsModel1.AgendaQuestionForAssignment)
                    .AddField(IssueGsModel1.AgendaQuestionAddress)
                    .AddField(IssueGsModel1.AgendaQuestionComment)
                    .AddField(IssueGsModel1.AgendaQuestionDescription)
                    .AddField(IssueGsModel1.AgendaQuestionExtResources)
                    .AddField(IssueGsModel1.AgendaQuestionIncomingDate)
                    .AddField(IssueGsModel1.AgendaQuestionInfo)
                    .AddField(IssueGsModel1.AgendaQuestionInvestor)
                    .AddField(IssueGsModel1.AgendaQuestionIsConsidered)
                    .AddField(IssueGsModel1.AgendaQuestionNumber)
                    .AddField(IssueGsModel1.AgendaQuestionProjectType)
                    .AddField(IssueGsModel1.AgendaQuestionProtocolDecision)
                    .AddField(IssueGsModel1.AgendaQuestionReason)
                    .AddField(IssueGsModel1.AgendaQuestionSiteName)
                    .AddField(IssueGsModel1.AgendaQuestionTheme)
                    .AddField(IssueGsModel1.CadastreNumber)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IssueGs, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IssueGsModel1.IssueMunicipalityGs,
                            IssueGsModel1.IssueSettlementGs,
                            IssueGsModel1.AgendaQuestionReporter,
                            IssueGsModel1.MeetingLink,
                            IssueGsModel1.AgendaQuestionDeclarant,
                            IssueGsModel1.QuestionCategoryLink,
                            IssueGsModel1.IssueGsIssueP,
                            IssueGsModel1.AgendaLinkedQuestionLink,
                            IssueGsModel1.AgendaQuestionCoreporter,
                            IssueGsModel1.AgendaQuestionObjectType,
                            IssueGsModel1.AgendaQuestionDecisionType,
                        //IssueGsModel1.MeetingDate,
                        //IssueGsModel1.MeetingDateText,
                        //IssueGsModel1.AgendaQuestionDeclarantId,
                        //IssueGsModel1.AgendaQuestionForAssignment,
                            IssueGsModel1.AgendaQuestionAddress,
                            IssueGsModel1.AgendaQuestionComment,
                            IssueGsModel1.AgendaQuestionDescription,
                            IssueGsModel1.AgendaQuestionExtResources,
                            IssueGsModel1.AgendaQuestionIncomingDate,
                            IssueGsModel1.AgendaQuestionInfo,
                            IssueGsModel1.AgendaQuestionInvestor,
                            IssueGsModel1.AgendaQuestionIsConsidered,
                            IssueGsModel1.AgendaQuestionNumber,
                            IssueGsModel1.AgendaQuestionProjectType,
                            IssueGsModel1.AgendaQuestionProtocolDecision,
                            IssueGsModel1.AgendaQuestionReason,
                            IssueGsModel1.AgendaQuestionSiteName,
                            IssueGsModel1.AgendaQuestionTheme,
                            IssueGsModel1.CadastreNumber
                            )
                    ));
        }

        protected void AddIssueGsContentTypes(ModelNode modelNode)
        {
            List issueGsList = AllLists.Single(s => s.RootFolder.Name == "AgendaQuestionList");
            List municipalityList = AllLists.Single(s => s.RootFolder.Name == ListModel.Municipality.Url);
            List participantList = AllLists.Single(s => s.RootFolder.Name == "ParticipantBookList");
            List meetingList = AllLists.Single(s => s.RootFolder.Name == "MeetingList");
            List organizationList = AllLists.Single(s => s.RootFolder.Name == "OrganizationBookList");
            List issuePList = AllLists.Single(s => s.RootFolder.Name == "IssuePList");
            List categoryList = AllLists.Single(s => s.RootFolder.Name == "AgendaQuestionCategoryBookList");
            List objectTypeList = AllLists.Single(s => s.RootFolder.Name == "List2");
            List decisionTypeList = AllLists.Single(s => s.RootFolder.Name == "DecisionTypeBookList");

            modelNode
                .WithFields(fields => fields
                    .AddField(IssueGsModel.IssueMunicipalityGs, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, municipalityList.Id, IssueGsModel.IssueMunicipalityGs.ShowField)))
                    .AddField(IssueGsModel.IssueSettlementGs, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, municipalityList.Id, IssueGsModel.IssueSettlementGs.ShowField)))
                    .AddField(IssueGsModel.AgendaQuestionReporter, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, participantList.Id, IssueGsModel.AgendaQuestionReporter.ShowField)))
                    .AddField(IssueGsModel.MeetingLink, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id, IssueGsModel.MeetingLink.ShowField)))
                    .AddField(IssueGsModel.AgendaQuestionDeclarant, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, organizationList.Id, IssueGsModel.AgendaQuestionDeclarant.ShowField)))
                    .AddField(IssueGsModel.QuestionCategoryLink, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, categoryList.Id, IssueGsModel.QuestionCategoryLink.ShowField)))
                    .AddField(IssueGsModel.IssueGsIssueP, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issuePList.Id, IssueGsModel.IssueGsIssueP.ShowField)))
                    .AddField(IssueGsModel.AgendaLinkedQuestionLink, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueGsList.Id, IssueGsModel.AgendaLinkedQuestionLink.ShowField)))
                    .AddField(IssueGsModel.AgendaQuestionCoreporter, field =>  //MULTI
                        field.OnCreated((fieldDef, spField) => {
                            spField.MakeLookupConnectionToList(Context.Web.Id, participantList.Id, IssueGsModel.AgendaQuestionCoreporter.ShowField);
                            ((FieldLookup)spField).AllowMultipleValues = true;
                        }))
                    .AddField(IssueGsModel.AgendaQuestionObjectType, field =>  //MULTI
                        field.OnCreated((fieldDef, spField) => {
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectTypeList.Id, IssueGsModel.AgendaQuestionObjectType.ShowField);
                            ((FieldLookup)spField).AllowMultipleValues = true;
                        }))
                    .AddField(IssueGsModel.AgendaQuestionDecisionType, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, decisionTypeList.Id, IssueGsModel.AgendaQuestionDecisionType.ShowField)))
                    //.AddField(IssueGsModel.MeetingDate, field =>
                    //    field.OnCreated((fieldDef, spField) =>
                    //        spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id, IssueGsModel.MeetingDate.ShowField)))
                    //.AddField(IssueGsModel.MeetingDateText, field =>
                    //    field.OnCreated((fieldDef, spField) =>
                    //        spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id, IssueGsModel.MeetingDateText.ShowField)))
                    //.AddField(IssueGsModel.AgendaQuestionDeclarantId, field =>
                    //    field.OnCreated((fieldDef, spField) =>
                    //        spField.MakeLookupConnectionToList(Context.Web.Id, organizationList.Id, IssueGsModel.AgendaQuestionDeclarantId.ShowField)))
                    //.AddField(IssueGsModel.AgendaQuestionForAssignment)
                    .AddField(IssueGsModel.AgendaQuestionAddress)
                    .AddField(IssueGsModel.AgendaQuestionComment)
                    .AddField(IssueGsModel.AgendaQuestionDescription)
                    .AddField(IssueGsModel.AgendaQuestionExtResources)
                    .AddField(IssueGsModel.AgendaQuestionIncomingDate)
                    .AddField(IssueGsModel.AgendaQuestionInfo)
                    .AddField(IssueGsModel.AgendaQuestionInvestor)
                    .AddField(IssueGsModel.AgendaQuestionIsConsidered)
                    .AddField(IssueGsModel.AgendaQuestionNumber)
                    .AddField(IssueGsModel.AgendaQuestionProjectType)
                    .AddField(IssueGsModel.AgendaQuestionProtocolDecision)
                    .AddField(IssueGsModel.AgendaQuestionReason)
                    .AddField(IssueGsModel.AgendaQuestionSiteName)
                    .AddField(IssueGsModel.AgendaQuestionTheme)
                    .AddField(IssueGsModel.CadastreNumber)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IssueGs, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IssueGsModel.IssueMunicipalityGs,
                            IssueGsModel.IssueSettlementGs,
                            IssueGsModel.AgendaQuestionReporter,
                            IssueGsModel.MeetingLink,
                            IssueGsModel.AgendaQuestionDeclarant,
                            IssueGsModel.QuestionCategoryLink,
                            IssueGsModel.IssueGsIssueP,
                            IssueGsModel.AgendaLinkedQuestionLink,
                            IssueGsModel.AgendaQuestionCoreporter,
                            IssueGsModel.AgendaQuestionObjectType,
                            IssueGsModel.AgendaQuestionDecisionType,
                            //IssueGsModel.MeetingDate,
                            //IssueGsModel.MeetingDateText,
                            //IssueGsModel.AgendaQuestionDeclarantId,
                            //IssueGsModel.AgendaQuestionForAssignment,
                            IssueGsModel.AgendaQuestionAddress,
                            IssueGsModel.AgendaQuestionComment,
                            IssueGsModel.AgendaQuestionDescription,
                            IssueGsModel.AgendaQuestionExtResources,
                            IssueGsModel.AgendaQuestionIncomingDate,
                            IssueGsModel.AgendaQuestionInfo,
                            IssueGsModel.AgendaQuestionInvestor,
                            IssueGsModel.AgendaQuestionIsConsidered,
                            IssueGsModel.AgendaQuestionNumber,
                            IssueGsModel.AgendaQuestionProjectType,
                            IssueGsModel.AgendaQuestionProtocolDecision,
                            IssueGsModel.AgendaQuestionReason,
                            IssueGsModel.AgendaQuestionSiteName,
                            IssueGsModel.AgendaQuestionTheme,
                            IssueGsModel.CadastreNumber)
                    ));
        }
        #endregion

        #region Lists
        protected void AddIgHistoryList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IgHistory,
                            list => list.AddContentTypeLink(ContentTypeModel.IgHistory)));
        }

        protected void AddIgMessageList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IgMessage,
                            list => list.AddContentTypeLink(ContentTypeModel.IgMessage)));
        }

        protected void AddMunicipalityList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.Municipality,
                            list => list.AddContentTypeLink(ContentTypeModel.Municipality)));
        }

        protected void AddConfigurationList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.Configuration,
                            list => list.AddContentTypeLink(ContentTypeModel.Configuration)));
        }

        protected void AddStatusList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.Status,
                            list => list.AddContentTypeLink(ContentTypeModel.Status)));
        }

        protected void AddBuilderList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.Builder,
                            list => list.AddContentTypeLink(ContentTypeModel.Builder)));
        }

        protected void AddIssueGsList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IssueGs,
                            list => list.AddContentTypeLink(ContentTypeModel.IssueGs)));
        }
        #endregion

        protected void CleanListContentTypes(string listName, string contentTypeName)
        {
            var query = Context.Web.Lists.Where(s => s.RootFolder.Name == listName)
                .Include(c => c.ContentTypes, c => c.RootFolder, c => c.RootFolder.ContentTypeOrder);

            IEnumerable<List> lists = Context.LoadQuery(query);
            Context.ExecuteQuery();
            List list = lists.First();

            ContentType targetType = null;
            var allContentTypes = new List<ContentType>();
            foreach (ContentType ct in list.ContentTypes)
            {
                allContentTypes.Add(ct);
                if (ct.Name.Equals(contentTypeName, StringComparison.OrdinalIgnoreCase))
                    targetType = ct;
            }

            list.RootFolder.UniqueContentTypeOrder = list.RootFolder.ContentTypeOrder.Where(s => s.StringValue == targetType.Id.StringValue).ToList();
            list.RootFolder.Update();
            Context.ExecuteQuery();

            foreach (ContentType ct in allContentTypes)
                if (!ct.Sealed && ct.Id.StringValue != targetType.Id.StringValue)
                    ct.DeleteObject();

            Context.ExecuteQuery();
        }
    }
}
