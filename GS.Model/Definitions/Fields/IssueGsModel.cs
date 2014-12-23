using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using System;

namespace GS.Model.Definitions.Fields
{
    public static class IssueGsModel
    {
        public static readonly string GroupName = Constants.FormName("Вопросы заседания");

        public static FieldDefinition IssueMunicipalityGs = new FieldDefinition
        {
            InternalName = "IssueMunicipalityGs",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{AD9EF609-544E-4A98-9537-DEC03D9488B6}"),
            Title = "Муниципальный район/Городской округ"
        };

        public static FieldDefinition IssueSettlementGs = new FieldDefinition
        {
            InternalName = "IssueSettlementGs",
            Description = "",
            FieldType = FieldType.Lookup.ToString(),
            Group = GroupName,
            Id = new Guid("{C4795764-E8F9-493E-981C-B5745E7B6A35}"),
            Title = "Поселение"
        };
    }
}
