﻿using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class IssueGsModel
    {
        public static readonly string GroupName = Constants.FormName("Вопросы заседания");

        public static FieldDefinition AgendaQuestionAddress = new FieldDefinition()
        {
            InternalName = "AgendaQuestionAddress",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{A7D035CA-FB43-422A-94EE-D2D1BBFE4085}"),
            Title = "Адрес"
        };

        public static FieldDefinition AgendaQuestionExtResources = new FieldDefinition()
        {
            InternalName = "AgendaQuestionExtResources",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{00736B47-92B8-4937-B832-1791FC8D5D58}"),
            Title = "Внешние источники"
        };

        public static FieldDefinition AgendaQuestionIncomingDate = new FieldDefinition()
        {
            InternalName = "AgendaQuestionIncomingDate",
            Description = "",
            FieldType = FieldType.DateTime.ToString(),
            Group = GroupName,
            Id = new Guid("{3631F7BA-209E-4C5E-9F6F-B62ED962C583}"),
            Title = "Дата рассмотрения"
        };

        public static FieldDefinition AgendaQuestionForAssignment = new FieldDefinition()
        {
            InternalName = "_x0414__x043b__x044f__x0020__x04",
            Description = "",
            FieldType = FieldType.Calculated.ToString(),
            Group = GroupName,
            Id = new Guid("{B7403452-4F8D-4B18-9192-E7CA83875A45}"),
            Title = "Для отображения в поручении"
        };

        public static FieldDefinition AgendaQuestionReporter = new FieldDefinition()
        {
            InternalName = "AgendaQuestionReporter",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{66E189EF-2888-4FBC-B315-849E618A0600}"),
            Title = "Докладчик",
            ShowField = "ParticipantFullName"
        };

        public static FieldDefinition MeetingLink = new FieldDefinition()
        {
            InternalName = "MeetingLink",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{5E394001-D439-4F43-84CA-B60FB90E225F}"),
            Title = "Заседание",
            ShowField = "ParticipantFullName"
        };

        public static FieldDefinition MeetingDate = new FieldDefinition()
        {
            InternalName = "_x0417__x0430__x0441__x0435__x04",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{9197215E-2FF3-4F74-BB8E-A32E875C8A48}"),
            Title = "Заседание:Дата",
            ShowField = "_x0414__x0430__x0442__x0430_",
            FieldRefId = MeetingLink.Id
        };

        public static FieldDefinition MeetingDateText = new FieldDefinition()
        {
            InternalName = "_x0417__x0430__x0441__x0435__x040",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{FD6D47F2-DA68-43B3-84F7-3B1A0914D189}"),
            Title = "Заседание:Дата проведения",
            ShowField = "MeetingDate",
            FieldRefId = MeetingLink.Id
        };

        public static FieldDefinition AgendaQuestionDeclarant = new FieldDefinition()
        {
            InternalName = "AgendaQuestionDeclarant",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{2895BEE0-7C62-4D4F-89C5-A08BF52DF5C9}"),
            Title = "Заявитель на комиссию",
            ShowField = "Title"
        };

        public static FieldDefinition AgendaQuestionDeclarantId = new FieldDefinition()
        {
            InternalName = "_x0417__x0430__x044f__x0432__x04",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{2895BEE0-7C62-4D4F-89C5-A08BF52DF5C9}"),
            Title = "Заявитель на комиссию:ИД",
            ShowField = "ИД",
            FieldRefId = AgendaQuestionDeclarant.Id
        };

        public static FieldDefinition AgendaQuestionInvestor = new FieldDefinition()
        {
            InternalName = "AgendaQuestionInvestor",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{60CA6290-63EF-4E14-8808-329172A5E615}"),
            Title = "Инвестор"
        };

        public static FieldDefinition AgendaQuestionInfo = new FieldDefinition()
        {
            InternalName = "_x0418__x043d__x0444__x043e_",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{89F3CF2D-82F1-447F-BFED-DC67FB653F94}"),
            Title = "Инфо"
        };

        public static FieldDefinition CadastreNumber = new FieldDefinition()
        {
            InternalName = "CadastreNumber",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{4A0FB6C5-6CCE-4A55-8E82-D7DA49230FD5}"),
            Title = "Кадастровый номер"
        };

        public static FieldDefinition QuestionCategoryLink = new FieldDefinition()
        {
            InternalName = "QuestionCategoryLink",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{3512E463-71E5-4F54-BA81-76C7D3FEEACD}"),
            Title = "Категория вопроса",
            ShowField = "QuestionCategoryName"
        };

        public static FieldDefinition AgendaQuestionComment = new FieldDefinition()
        {
            InternalName = "AgendaQuestionComment",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{5314F17A-12B7-494D-96AD-9094745B3FC1}"),
            Title = "Комментарий"
        };

        public static FieldDefinition IssueMunicipalityGs = new FieldDefinition
        {
            InternalName = "IssueMunicipalityGs",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{AD9EF609-544E-4A98-9537-DEC03D9488B6}"),
            Title = "Муниципальный район/Городской округ"
        };

        public static FieldDefinition AgendaQuestionSiteName = new FieldDefinition()
        {
            InternalName = "AgendaQuestionSiteName",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{37390CA0-94A7-44EE-8A5B-65ED93FEDDDA}"),
            Title = "Наименование объекта"
        };

        public static FieldDefinition AgendaQuestionNumber = new FieldDefinition()
        {
            InternalName = "AgendaQuestionNumber",
            Description = "",
            FieldType = FieldType.Number.ToString(),
            Group = GroupName,
            Id = new Guid("{EEA7C7EB-4A28-4EEF-9984-E64217549866}"),
            Title = "Номер вопроса"
        };

        public static FieldDefinition AgendaQuestionDescription = new FieldDefinition()
        {
            InternalName = "AgendaQuestionDescription",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{A541AB65-CFCF-4290-8425-C39FF4E20FE8}"),
            Title = "Описание вопроса повестки"
        };

        public static FieldDefinition AgendaQuestionReason = new FieldDefinition()
        {
            InternalName = "AgendaQuestionReason",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{CC9004AC-6867-406F-A4BE-452ACC41F4A4}"),
            Title = "Основание"
        };

        public static FieldDefinition IssueGsIssueP = new FieldDefinition
        {
            InternalName = "IssueGsIssueP",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{D8CB2528-AA57-426A-850D-A5716C34D7C2}"),
            Title = "Плановый вопрос",
            ShowField = "ИД"
        };

        public static FieldDefinition IssueSettlementGs = new FieldDefinition
        {
            InternalName = "IssueSettlementGs",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{C4795764-E8F9-493E-981C-B5745E7B6A35}"),
            Title = "Поселение",
            ShowField = "Title"
        };

        public static FieldDefinition AgendaQuestionIsConsidered = new FieldDefinition()
        {
            InternalName = "AgendaQuestionIsConsidered",
            Description = "",
            FieldType = FieldType.Boolean.ToString(),
            Group = GroupName,
            Id = new Guid("{86D4CA1C-4837-4A66-961C-5D5F0F68A42C}"),
            Title = "Рассмотрен"
        };

        public static FieldDefinition AgendaQuestionProtocolDecision = new FieldDefinition()
        {
            InternalName = "AgendaQuestionProtocolDecision",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{8B2783E5-782B-45D4-9482-4F9E6180CAAD}"),
            Title = "Решение"
        };

        public static FieldDefinition AgendaLinkedQuestionLink = new FieldDefinition
        {
            InternalName = "AgendaLinkedQuestionLink",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{EB835C00-436F-479F-AF3F-1EAF097C21CB}"),
            Title = "Связанный вопрос",
            ShowField = "AgendaQuestionNumber"
        };

        public static FieldDefinition AgendaQuestionCoreporter = new FieldDefinition
        {
            InternalName = "AgendaQuestionCoreporter",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{EDE62C9B-FE68-4082-9421-DAB3F556C58D}"),
            Title = "Содокладчики",
            ShowField = "ParticipantFullName"
        };

        public static FieldDefinition AgendaQuestionTheme = new FieldDefinition()
        {
            InternalName = "AgendaQuestionTheme",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{83B3D4E2-E519-466D-BD5C-346935322DC2}"),
            Title = "Тема вопроса"
        };

        public static FieldDefinition AgendaQuestionObjectType = new FieldDefinition
        {
            InternalName = "_x0422__x0438__x043f__x0020__x04",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{7C058DFD-5D39-4196-A5A1-877A58C8CBD6}"),
            Title = "Тип объекта",
            ShowField = "Title"
        };

        public static FieldDefinition AgendaQuestionProjectType = new FieldDefinition()
        {
            InternalName = "AgendaQuestionProjectType",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{E8641ABF-3767-4F00-AB8E-E21B94F69EA1}"),
            Title = "Тип проекта"
        };

        public static FieldDefinition AgendaQuestionDecisionType = new FieldDefinition
        {
            InternalName = "AgendaQuestionDecisionType",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{CF5F99E1-7CDD-4F21-97C6-D62D48BF303D}"),
            Title = "Тип решения",
            ShowField = "Title"
        };
    }
}
