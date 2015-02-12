using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class IssueGsModel1
    {
        public static readonly string GroupName = Constants.FormName("Вопросы заседания");

        public static FieldDefinition AgendaQuestionAddress = new FieldDefinition()
        {
            InternalName = "AgendaQuestionAddress1",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{E02EBF38-6203-49AD-BDBB-32728536B886}"),
            Title = "Адрес1"
        };

        public static FieldDefinition AgendaQuestionExtResources = new FieldDefinition()
        {
            InternalName = "AgendaQuestionExtResources1",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{5BD35C39-AD25-4BAB-9C6B-9595A1115B85}"),
            Title = "Внешние источники1"
        };

        public static FieldDefinition AgendaQuestionIncomingDate = new FieldDefinition()
        {
            InternalName = "AgendaQuestionIncomingDate1",
            Description = "",
            FieldType = FieldType.DateTime.ToString(),
            Group = GroupName,
            Id = new Guid("{B8BEE72F-57A7-4402-9625-2E646EFEB155}"),
            Title = "Дата рассмотрения1"
        };

        public static FieldDefinition AgendaQuestionForAssignment = new FieldDefinition()
        {
            InternalName = "AgendaQuestionForAssignment",
            Description = "",
            FieldType = FieldType.Calculated.ToString(),
            Group = GroupName,
            Id = new Guid("{65C450F2-C143-4EAB-9960-FBF6EF0BA4A6}"),
            Title = "Для отображения в поручении1"
        };

        public static FieldDefinition AgendaQuestionReporter = new FieldDefinition()
        {
            InternalName = "AgendaQuestionReporter1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{A795B79F-0FAC-46F0-8EEA-57E7A53FE8B8}"),
            Title = "Докладчик1",
            ShowField = "ParticipantFullName"
        };

        public static FieldDefinition MeetingLink = new FieldDefinition()
        {
            InternalName = "MeetingLink1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{3927D16A-C632-464F-8063-4471701A510B}"),
            Title = "Заседание1",
            ShowField = "ParticipantFullName"
        };

        public static FieldDefinition MeetingDate = new FieldDefinition()
        {
            InternalName = "MeetingDate",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{EEB3C0AE-D516-4841-9F57-DFFA97B37044}"),
            Title = "Заседание:Дата1",
            ShowField = "_x0414__x0430__x0442__x0430_",
            FieldRefId = MeetingLink.Id
        };

        public static FieldDefinition MeetingDateText = new FieldDefinition()
        {
            InternalName = "MeetingDateText",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{FBE4DD70-0DAF-4E37-863F-5455A47FE115}"),
            Title = "Заседание:Дата проведения1",
            ShowField = "MeetingDate",
            FieldRefId = MeetingLink.Id
        };

        public static FieldDefinition AgendaQuestionDeclarant = new FieldDefinition()
        {
            InternalName = "AgendaQuestionDeclarant1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{2F02AB4C-2BB9-49A3-9BEE-068434606D0E}"),
            Title = "Заявитель на комиссию1",
            ShowField = "Title"
        };

        public static FieldDefinition AgendaQuestionDeclarantId = new FieldDefinition()
        {
            InternalName = "AgendaQuestionDeclarantId",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{5A1ACAEE-5A2C-45F8-9399-2782DFE1EC05}"),
            Title = "Заявитель на комиссию:ИД1",
            ShowField = "ID",
            FieldRefId = AgendaQuestionDeclarant.Id
        };

        public static FieldDefinition AgendaQuestionInvestor = new FieldDefinition()
        {
            InternalName = "AgendaQuestionInvestor1",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{13FD9BBD-82D5-4C6A-B8DD-C675451715AE}"),
            Title = "Инвестор1"
        };

        public static FieldDefinition AgendaQuestionInfo = new FieldDefinition()
        {
            InternalName = "AgendaQuestionInfo",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{BDFE5B97-1D16-4D73-B382-9F17ED2E501E}"),
            Title = "Инфо1"
        };

        public static FieldDefinition CadastreNumber = new FieldDefinition()
        {
            InternalName = "CadastreNumber1",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{634CB948-67D3-4E48-8BE2-CA3898008617}"),
            Title = "Кадастровый номер1"
        };

        public static FieldDefinition QuestionCategoryLink = new FieldDefinition()
        {
            InternalName = "QuestionCategoryLink1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{F289CB52-A8DC-4323-97BB-0EB01537F816}"),
            Title = "Категория вопроса1",
            ShowField = "QuestionCategoryName"
        };

        public static FieldDefinition AgendaQuestionComment = new FieldDefinition()
        {
            InternalName = "AgendaQuestionComment1",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{3A347EB1-3F3C-4769-B08B-1E43B3EA81DB}"),
            Title = "Комментарий1"
        };

        public static FieldDefinition IssueMunicipalityGs = new FieldDefinition
        {
            InternalName = "IssueMunicipalityGs1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{163FD684-FF32-43A4-B13A-77B2ACE778EC}"),
            Title = "Муниципальный район/Городской округ1"
        };

        public static FieldDefinition AgendaQuestionSiteName = new FieldDefinition()
        {
            InternalName = "AgendaQuestionSiteName1",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{1B0C207F-8112-49DE-847B-7F32E95325C0}"),
            Title = "Наименование объекта1"
        };

        public static FieldDefinition AgendaQuestionNumber = new FieldDefinition()
        {
            InternalName = "AgendaQuestionNumber1",
            Description = "",
            FieldType = FieldType.Number.ToString(),
            Group = GroupName,
            Id = new Guid("{1C6C29CA-30E7-4E04-8238-629BB4066D18}"),
            Title = "Номер вопроса1"
        };

        public static FieldDefinition AgendaQuestionDescription = new FieldDefinition()
        {
            InternalName = "AgendaQuestionDescription1",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{FC5EF1F9-113C-4692-A78E-CF6687D36ACA}"),
            Title = "Описание вопроса повестки1"
        };

        public static FieldDefinition AgendaQuestionReason = new FieldDefinition()
        {
            InternalName = "AgendaQuestionReason1",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{3FEA06DC-022E-4626-9D23-76641BB61373}"),
            Title = "Основание1"
        };

        public static FieldDefinition IssueGsIssueP = new FieldDefinition
        {
            InternalName = "IssueGsIssueP1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{A68EC45E-74D5-4294-9318-92F75BBF0B5C}"),
            Title = "Плановый вопрос1",
            ShowField = "ИД"
        };

        public static FieldDefinition IssueSettlementGs = new FieldDefinition
        {
            InternalName = "IssueSettlementGs1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{A8AFDC7B-B902-47AD-AB57-9E5E52A482DB}"),
            Title = "Поселение1",
            ShowField = "Title"
        };

        public static FieldDefinition AgendaQuestionIsConsidered = new FieldDefinition()
        {
            InternalName = "AgendaQuestionIsConsidered1",
            Description = "",
            FieldType = FieldType.Boolean.ToString(),
            Group = GroupName,
            Id = new Guid("{CFF28D13-EDF7-46BA-98D1-44DA9C391B63}"),
            Title = "Рассмотрен1"
        };

        public static FieldDefinition AgendaQuestionProtocolDecision = new FieldDefinition()
        {
            InternalName = "AgendaQuestionProtocolDecision1",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{1E01249F-21D5-4A97-8AEC-4138C67E92AA}"),
            Title = "Решение1"
        };

        public static FieldDefinition AgendaLinkedQuestionLink = new FieldDefinition
        {
            InternalName = "AgendaLinkedQuestionLink1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{FCBAA0ED-B714-46BB-B0A9-E6A3712679EC}"),
            Title = "Связанный вопрос1",
            ShowField = "AgendaQuestionNumber"
        };

        public static FieldDefinition AgendaQuestionCoreporter = new FieldDefinition
        {
            InternalName = "AgendaQuestionCoreporter1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{A00DD88B-EF5C-49CF-ABD4-0737BAFD57FD}"),
            Title = "Содокладчики1",
            ShowField = "ParticipantFullName"
        };

        public static FieldDefinition AgendaQuestionTheme = new FieldDefinition()
        {
            InternalName = "AgendaQuestionTheme1",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{9E8E3322-5432-49C7-AE76-AA5FC4EC647C}"),
            Title = "Тема вопроса1"
        };

        public static FieldDefinition AgendaQuestionObjectType = new FieldDefinition
        {
            InternalName = "AgendaQuestionObjectType",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{42106A6B-B987-4BAF-87C1-7E9BCF9DCF11}"),
            Title = "Тип объекта1",
            ShowField = "Title"
        };

        public static FieldDefinition AgendaQuestionProjectType = new FieldDefinition()
        {
            InternalName = "AgendaQuestionProjectType1",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{7ED6DA4D-8E96-44C8-B364-3AE914A4783B}"),
            Title = "Тип проекта1"
        };

        public static FieldDefinition AgendaQuestionDecisionType = new FieldDefinition
        {
            InternalName = "AgendaQuestionDecisionType1",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{69555A8E-C398-4591-B515-CC973B1B32E5}"),
            Title = "Тип решения1",
            ShowField = "Title"
        };
    }
}
