using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class ConfigurationModel
    {
        public static readonly string GroupName = Constants.FormName("Конфигурация");

        public static FieldDefinition ConfigurationKey = new FieldDefinition
        {
            InternalName = "ConfigurationKey",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{D2CF41D9-3F78-4CBD-944F-40129673B4BF}"),
            Title = "Ключ"
        };

        public static FieldDefinition ConfigurationGroup = new FieldDefinition
        {
            InternalName = "ConfigurationGroup",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{F1B08F0D-C614-451D-B3ED-55BB98AD50D7}"),
            Title = "Группа"
        };

        public static FieldDefinition ConfigurationValue = new FieldDefinition
        {
            InternalName = "ConfigurationValue",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{5A001C0A-6CF3-4EB2-B7C4-BF6BF974CA54}"),
            Title = "Значение"
        };

        public static FieldDefinition ConfigurationParent = new FieldDefinition
        {
            InternalName = "ConfigurationParent",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{B2785301-F128-4DAD-B23E-5E09261B1B9A}"),
            Title = "Родитель"
        };
    }
}
