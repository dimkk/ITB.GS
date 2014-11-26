using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;

namespace GS.Model.Definitions
{
    public static class ContentTypeModel
    {
        public static readonly string GroupName = Constants.SystemName;

        public static ContentTypeDefinition IgHistory = new ContentTypeDefinition
        {
            Name = "IgHistory",
            Description = "",
            Group = GroupName,
            Id = new Guid("{CD89522D-9D8B-4690-BE2D-465C04603648}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };

        public static ContentTypeDefinition IgMessage = new ContentTypeDefinition
        {
            Name = "IgMessage",
            Description = "",
            Group = GroupName,
            Id = new Guid("{81F4E229-2C36-4620-B7C1-D5E8FDC86193}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };

        public static ContentTypeDefinition Municipality = new ContentTypeDefinition
        {
            Name = "Municipality",
            Description = "",
            Group = GroupName,
            Id = new Guid("{786A6077-A17C-49E5-9430-98935CBC5EFB}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };

        public static ContentTypeDefinition Configuration = new ContentTypeDefinition
        {
            Name = "Configuration",
            Description = "",
            Group = GroupName,
            Id = new Guid("{631D32AD-E91C-4864-9F29-94A1DA3B6740}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };

        public static ContentTypeDefinition Status = new ContentTypeDefinition
        {
            Name = "Status",
            Description = "",
            Group = GroupName,
            Id = new Guid("{AD72B250-C9D9-4DD8-B1A8-24F02A9395AE}"),
            ParentContentTypeId = BuiltInSiteContentTypeId.Item
        };
    }
}
