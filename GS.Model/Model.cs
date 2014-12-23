﻿using Microsoft.SharePoint.Client;
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
            AddIssueGsContentTypes(siteModel);

            Service.DeployModel(siteModelHost, siteModel);

            ModelNode webModel = SPMeta2Model.NewWebModel(new WebDefinition() { RequireSelfProcessing = false });

            //Добавляем списки
            //AddIgHistoryList(webModel);
            //AddIgMessageList(webModel);
            //AddMunicipalityList(webModel);
            //AddConfigurationList(webModel);
            //AddStatusList(webModel);
            //AddBuilderList(webModel);

            Service.DeployModel(webModelHost, webModel);

            //CleanListContentTypes(ListModel.IgHistory.Url, ContentTypeModel.IgHistory.Name);
            //CleanListContentTypes(ListModel.IgMessage.Url, ContentTypeModel.IgMessage.Name);
            //CleanListContentTypes(ListModel.Municipality.Url, ContentTypeModel.Municipality.Name);
            //CleanListContentTypes(ListModel.Configuration.Url, ContentTypeModel.Configuration.Name);
            //CleanListContentTypes(ListModel.Status.Url, ContentTypeModel.Status.Name);
            //CleanListContentTypes(ListModel.Builder.Url, ContentTypeModel.Builder.Name);
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

        protected void AddIssueGsContentTypes(ModelNode modelNode)
        {
            List municipalityList = AllLists.Single(s => s.RootFolder.Name == ListModel.Municipality.Url);

            modelNode
                .WithFields(fields => fields
                    .AddField(IssueGsModel.IssueMunicipalityGs, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, municipalityList.Id, "Title")))
                    .AddField(IssueGsModel.IssueSettlementGs, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, municipalityList.Id, "Title")))
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IssueGs, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IssueGsModel.IssueMunicipalityGs,
                            IssueGsModel.IssueSettlementGs)
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
