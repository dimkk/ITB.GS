using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using ITB.SP.Tools;

namespace GS.Land.ModelXml.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e03c5ed6-3ac8-4000-bf0b-65791c013ebc")]
    public class IssueCategoryListsEventReceiver : SPFeatureReceiver
    {
        #region Constants

        private readonly string fieldContentTypeName = "IssueCategoryLand";
        private readonly string fieldGuid = "{85B72DCD-02BC-46A6-B3B6-8C77743B9A8E}";
        private readonly static string fieldName = "IssueCategoryParentLand";

        private readonly string fieldParentFeatureId = "e2d4932e-0cc5-47ab-a254-25ac16ba1d0f";
        private readonly string fieldGroupName = "ГС.Земля.Категории вопросов";
        private readonly string fieldDisplayName = "Родительская категория";
        private readonly string fieldDescription = "Ссылка на родительскую категорию";

        private readonly string targetShowFieldName = "Title";
        private readonly string targetLookupListRelativeUrl = "IssueCategoryLandList";
        #endregion

        #region Items
        private readonly List<Dictionary<string, object>> items = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>()
            {
                { "Title", "Организационные вопросы" }  
            },
            new Dictionary<string, object>()
            {
                { "Title", "О приобретении в собственность Московской области земельных участков в порядке реализации преимущественного права покупки земельных участков  из земель сельскохозяйственного назначения" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Подготовка заключений в Градостроительный совет Московской области" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Об изъятии земельных участков и объектов недвижимого имущества для государственных нужд Московской области" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "О предоставлении земельных участков, находящихся в собственности Московской области, в аренду" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Об исполнении поручений Градостроительного совета Московской области" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "О переводе земельного участка из одной категории в другую" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Об изменении границ лесопарковых зон" }
            },
            new Dictionary<string, object>()
            {
                { "Title", "О ходе исполнения поручений заместителя председателя Правительства Московской области - А.А. Чупракова" },
                { fieldName, new SPFieldLookupValue(1, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты заключений о возможности и целесообразности включения земельных участков в границы населённых пунктов" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений об изменении (установлении) вида разрешенного использования земельных участков" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о согласовании мест размещения объектов (утверждение актов выбора земельного участка)" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о предоставлении земельных участков на праве постоянного (бессрочного) пользования" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о предоставлении земельных участков в порядке переоформления  постоянного (бессрочного) пользования" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о предоставлении земельных участков многодетным семьям" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о проведении аукционов по продаже права на заключение договоров аренды земельных участков" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о проведении аукционов по продаже земельных участков" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о продаже земельных участков или передаче в аренду собственникам объектами капитального строительства (ст. 36 ЗК РФ)" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о преимущественном праве покупки земельного участка, находящегося в аренде" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о предоставлении земельных участков в собственность или аренду для целей, не связанных со строительством" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о заключении договоров аренды, дополнительных соглашений к договорам аренды, о расторжении договоров аренды" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            },
            new Dictionary<string, object>()
            {
                { "Title", "Проекты решений о переводе земель, находящихся в частной собственности, из одной категории в другую" },
                { fieldName, new SPFieldLookupValue(3, string.Empty) }
            }
        };
        #endregion

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (var site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RootWeb.AddLookupField(fieldParentFeatureId, fieldContentTypeName, fieldGuid, fieldName, fieldGroupName, fieldDisplayName, fieldDescription, targetShowFieldName, targetLookupListRelativeUrl);
            }
            using (var web = ((SPSite)properties.Feature.Parent).RootWeb)
            {
                FillList(web.GetListByUrl("IssueCategoryLandList"), items);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                if (site == null)
                    throw new Exception("Feature must be activated at site collection level");

                site.RootWeb.DeleteField(fieldName);
            }
        }

        private void FillList(SPList list, IEnumerable<Dictionary<string, object>> items)
        {
            foreach (var item in items)
            {
                SPListItem listItem = list.AddItem();
                try
                {
                    foreach (var fieldName in item.Keys)
                        listItem[fieldName] = item[fieldName];
                    listItem.Update();
                }
                catch (Exception e)
                {
                    Log.Unexpected(e, "Не удалось сохранить элемент \"{0}\" списка {1}", listItem.Title, list.RootFolder);
                }
            }
        }
    }
}
