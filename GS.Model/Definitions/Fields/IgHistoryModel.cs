using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class IgHistoryModel
    {
        public static readonly string GroupName = Constants.FormName("Интеграция.История");

        public static FieldDefinition IgHistoryIssuePlan = new FieldDefinition
        {
            InternalName = "IgHistoryIssuePlan",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{99C5E0E0-0C54-4CBE-8221-1048E7533FD8}"),
            Title = "Плановый вопрос"
        };

        public static FieldDefinition IgHistoryDictionary = new FieldDefinition
        {
            InternalName = "IgHistoryDictionary",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{AE152DD6-B51A-4D1E-A231-5210C4B491C3}"),
            Title = "Справочник"
        };

        public static FieldDefinition IgHistoryDirection = new FieldDefinition
        {
            InternalName = "IgHistoryDirection",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{7BB19D05-8339-43F8-A08F-CD7C939DF7E5}"),
            Title = "Направление"
        };

        public static FieldDefinition IgHistorySenderSystem = new FieldDefinition
        {
            InternalName = "IgHistorySenderSystem",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{988C8455-FA44-4266-8F7C-71DB7480C829}"),
            Title = "Система-отправитель"
        };

        public static FieldDefinition IgHistoryReceiverSystem = new FieldDefinition
        {
            InternalName = "IgHistoryReceiverSystem",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{7E19DD4D-7C8F-4521-853D-19846F333129}"),
            Title = "Система-получатель"
        };

        public static FieldDefinition IgHistoryStatus = new FieldDefinition
        {
            InternalName = "IgHistoryStatus",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{327FCA25-25B5-432B-AE97-5946EEE2C5BC}"),
            Title = "Статус"
        };

        public static FieldDefinition IgHistoryError = new FieldDefinition
        {
            InternalName = "IgHistoryError",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{D9FD69AB-9F42-43FE-A6B6-B5E8445F1A73}"),
            Title = "Описание ошибки"
        };

        public static FieldDefinition IgHistorySendTryCount = new FieldDefinition
        {
            InternalName = "IgHistorySendTryCount",
            Description = "",
            FieldType = FieldType.Integer.ToString(),
            Group = GroupName,
            Id = new Guid("{E8D34977-EC68-48AC-A3D5-41E0558D61C3}"),
            Title = "Количество попыток отправки"
        };
    }
}
