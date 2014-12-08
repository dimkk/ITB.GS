using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class BuilderModel
    {
        public static readonly string GroupName = Constants.FormName("Застройщики");

        public static FieldDefinition BuilderInn = new FieldDefinition
        {
            InternalName = "BuilderInn",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{571FCA24-8318-4CA5-8030-BE1A98744806}"),
            Title = "ИНН"
        };

        public static FieldDefinition BuilderForm = new FieldDefinition
        {
            InternalName = "BuilderForm",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{AA72055E-A562-4FC6-90ED-DB07E65EF5AA}"),
            Title = "Организационно-правовая форма организации"
        };

        public static FieldDefinition BuilderLegalAddress = new FieldDefinition
        {
            InternalName = "BuilderLegalAddress",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{6B130F51-B810-4096-854B-FEA9A7AA6CFA}"),
            Title = "Юридический адрес"
        };

        public static FieldDefinition BuilderFactAddress = new FieldDefinition
        {
            InternalName = "BuilderFactAddress",
            Description = "",
            FieldType = FieldType.Note.ToString(),
            Group = GroupName,
            Id = new Guid("{62B4A6F4-4F2A-413E-8252-5FCC0565D647}"),
            Title = "Фактический адрес"
        };

        public static FieldDefinition BuilderParent = new FieldDefinition
        {
            InternalName = "BuilderParent",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{E77EAB2F-EFA3-4C53-8B6B-D611888A5720}"),
            Title = "Вышестоящая организация"
        };

        public static FieldDefinition BuilderExtId = new FieldDefinition
        {
            InternalName = "BuilderExtId",
            Description = "",
            FieldType = FieldType.Text.ToString(),
            Group = GroupName,
            Id = new Guid("{EE091682-EB18-4C0F-9E2F-039318854672}"),
            Title = "Внешний ID"
        };
    }
}
