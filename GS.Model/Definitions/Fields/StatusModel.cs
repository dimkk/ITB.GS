using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class StatusModel
    {
        public static readonly string GroupName = Constants.FormName("Статусы");

        public static FieldDefinition StatusKey = new FieldDefinition
        {
            InternalName = "StatusKey",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{62B2AE89-FF2C-43BB-A7BB-5A393B1F90B5}"),
            Title = "Ключ"
        };
    }
}
