using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;

namespace GS.Model.Definitions
{
    public static class ListModel
    {
        public static ListDefinition IgHistory = new ListDefinition
        {
            Title = "Интеграция - История",
            Url = "IgHistory",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition IgMessage = new ListDefinition
        {
            Title = "Интеграция - Сообщения",
            Url = "IgMessage",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition Municipality = new ListDefinition
        {
            Title = "Муниципальные образования",
            Url = "Municipality",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition Configuration = new ListDefinition
        {
            Title = "Конфигурация",
            Url = "Configuration",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };

        public static ListDefinition Status = new ListDefinition
        {
            Title = "Статусы",
            Url = "Status",
            TemplateType = (int)ListTemplateType.GenericList,
            Description = "",
            ContentTypesEnabled = true
        };
    }
}
