using CamlexNET;
using GS.Common.BL;
using ITB.Ig.Module.Sivgk.v1.Models;
using ITB.Ig.Module.Sivgk.v1.Repositories;
using ITB.SP.Tools;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GS.Ig.Module.Sivgk.v1.Repositories
{
    /// <summary>
    /// Репозитарий АК - Вопросы заседания 
    /// </summary>
    public class QuestionPlanRepository : QuestionRepositoryTP
    {
        private SPList _AttList;

        public QuestionPlanRepository(SPWeb oWeb)
            : base(oWeb)
        {
        }

        public Question Get(object id)
        {
            SPListItem item = null;
            //ищем по инту или гуиду
            if (id is int)
                item = GetByID((int)id);
            else                
                item = GetByID((Guid)id);
            if (item == null) return null;

            return GetModel(item);            
        }

        public override Question GetModel(SPListItem item)
        {
            var result = new Question()
            {
                Address = (string) item["Адрес"],
                Description = (string) item["Описание вопроса"],
                Investor = (string) item["Инвестор"],
                QuestionCategory =
                    item["Категория вопроса"] != null
                        ? new SPFieldLookupValue((string) item["Категория вопроса"]).LookupId
                        : (int?) null,
                MunicipalAreaID =
                    item["Муниципальный район/Городской округ"] != null
                        ? new SPFieldLookupValue((string)item["Муниципальный район/Городской округ"]).LookupId
                        : (int?) null,
                LocationID =
                    item["Поселение"] != null ? new SPFieldLookupValue((string) item["Поселение"]).LookupId : (int?) null,
                ID = (int) item["ID"],
                Attachments = GetAttachments((int) item["ID"]),
                GuidID = Guid.Parse((string) item["GUID"]),
                Url = item.GetDisplayUrl(),
                SourceEntityType = ESourceEntityType.PlanQuestion 
            };
            return result;
        }


        protected QuestionAttachment[] GetAttachments(int id)
        {
            //генерим запрос            
            return GetAttachmentItems(id)                
                .SelectMany(i => i.Folder.Files
                    .Cast<SPFile>()
                    .Select(f => new QuestionAttachment()
                        {
                            Content = f.OpenBinary(),
                            FileName = f.Name
                        }))
                .ToArray();
        }

        private IEnumerable<SPListItem> GetAttachmentItems(int id)
        {
            var query = Camlex.Query().Where(i => i[AttachmentRefFieldName] == (DataTypes.LookupId)id.ToString()).ToSPQuery();

            IEnumerable<SPListItem> spListItems = AttList.GetItems(query).Cast<SPListItem>();
            return spListItems;
        }

        public SPList AttList
        {
            get
            {
                if (_AttList == null)
                {
                    _AttList = _oWeb.Lists[AttachmtnListName];
                }
                return _AttList;
            }
        }

        protected virtual string AttachmtnListName
        {
            get { return "Вложения"; }
        }

//        public bool Save(Question item)
//        {
//            //ищем вопрос 
//            var spQuestion = GetByID(item.GuidID);
//            bool isNew = false;
//            if (spQuestion == null)
//            {
//                //не нашли - создаем
//                spQuestion = List.AddItem();
//                spQuestion["GUID"] = item.GuidID;
//                isNew = true;
//            }
//            //перекидываем значения
//            QuestionToSPItem(item, spQuestion);
//
//            if (item.Attachments != null && item.Attachments.Length > 0)
//            {
//                var attItem = GetAttachmentItems(spQuestion.ID).FirstOrDefault();
//                if (attItem == null)
//                {
//                    attItem = AttList.Items.Add(AttList.RootFolder.ServerRelativeUrl, SPFileSystemObjectType.Folder, "root");
//                    attItem.Update();
//                    attItem[AttachmentRefFieldName] = spQuestion.ID;                    
//                }
//
//                var files = attItem.Folder.Files.Cast<SPFile>().ToArray();
//
//                ///генерим приложения
//                item.Attachments.ToList().ForEach(a =>
//                {
//                    //ищем такой файл
//                    var file = files.FirstOrDefault(i => i.Name == a.FileName) ?? attItem.Folder.Files.Add(a.FileName, a.Content);                    
//                });
//            }
//
//#if !DEBUG
//            List.Update();
//#endif
//
//            return isNew;
//        }

        protected virtual string AttachmentRefFieldName
        {
            get { return "AttachmentIssueAk"; }
        }

        protected override void QuestionToSPItem(Question item, SPListItem spQuestion)
        {
            spQuestion["Адрес"] = item.Address;
            spQuestion["Описание вопроса"] = item.Description;
            spQuestion["Инвестор"] = item.Investor;
            spQuestion["Кадастровый номер"] = item.KadastrNumber;
            spQuestion["Категория вопроса"] = new SPFieldLookupValue(1, null);

            SPListItem municipalityItem = null;
            SPListItem settlementItem = null;
            if (item.MunicipalAreaID.HasValue)
                municipalityItem = MunicipalityRepository.GetItemByExtId(spQuestion.Web, item.MunicipalAreaID.Value.ToString());
            if (item.LocationID.HasValue)
                settlementItem = MunicipalityRepository.GetItemByExtId(spQuestion.Web, item.LocationID.Value.ToString());

            if (municipalityItem != null)
                spQuestion["IssueMunicipalityP"] = new SPFieldLookupValue() { LookupId = municipalityItem.ID };
            if (settlementItem != null)
                spQuestion["IssueSettlementP"] = new SPFieldLookupValue() {LookupId = settlementItem.ID};

            spQuestion["IssueSourceIdP"] = item.ID;
            spQuestion["IssueSourceTypeP"] = item.SourceEntityType;
            spQuestion["IssueSourceUrlP"] = item.Url;
            spQuestion["IssueSourceSystemP"] = item.SourceSystem;

            spQuestion["IssueStatusP"] = new SPFieldLookupValue(Status.GetIdByStatus(spQuestion.Web, StatusEnum.IgAdded), null);
        }

        public override string ListName
        {
            get { return "Плановые вопросы"; }
        }

    }
}