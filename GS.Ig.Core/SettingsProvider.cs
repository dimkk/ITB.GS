using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CamlexNET;
using GS.Ig.Core.Interfaces;
using ITB.SP.Tools;
using Microsoft.SharePoint;

namespace GS.Ig.Core
{
    public class SettingsProvider : ISettingsProvider
    {
        protected readonly SPWeb Web;
        protected readonly int? Id;

        protected readonly string ConfigListName = "Configuration";
        protected readonly string GroupFieldName = "ConfigurationGroup";
        protected readonly string ParentFieldName = "ConfigurationParent";
        protected readonly string KeyFieldName = "ConfigurationKey";
        protected readonly string ValueFieldName = "ConfigurationValue";

        public SettingsProvider(SPWeb web)
        {
            Web = web;
            Id = TryGetConfigItem("IG", true).ID;
            Id = TryGetConfigItem("Modules", true).ID;
        }

        public SettingsProvider(SPWeb web, SPListItem item)
            : this(web)
        {
            Id = item.ID;
        }

        protected SPListItem TryGetConfigItem(string key, bool throwException)
        {
            SPQuery query = Id.HasValue
                ? Camlex.Query()
                    .Where(
                        x =>
                            x[ParentFieldName] == (DataTypes.LookupId)Id.Value.ToString() &&
                            x[KeyFieldName] == (DataTypes.Text)key)
                    .ToSPQuery()
                : Camlex.Query()
                    .Where(x => x[KeyFieldName] == (DataTypes.Text)key)
                    .ToSPQuery();
            SPListItem item = Web.GetListItems(ConfigListName, query).SingleOrDefault();
            if (throwException && item == null)
                throw new Exception(string.Format("В списке {0} не найден параметр конфигурации (Key = '{1}')", ConfigListName, key));
            return item;
        }

        public IEnumerable<ISettingsProvider> GetAll()
        {
            SPQuery query = Id.HasValue
                ? Camlex.Query().Where(x => x[ParentFieldName] == (DataTypes.LookupId)Id.Value.ToString()).ToSPQuery()
                : Camlex.Query().ToSPQuery();
            return Web.GetListItems(ConfigListName, query).Select(s => new SettingsProvider(Web, s));
        }

        public ISettingsProvider Get(string key)
        {
            return new SettingsProvider(Web, TryGetConfigItem(key, true));
        }

        public string this[string key]
        {
            get
            {
                return TryGetConfigItem(key, true).GetFieldValue<string>(ValueFieldName);
            }
            set
            {
                SPListItem item = TryGetConfigItem(key, false);
                if (item == null)
                {
                    item = Web.GetListByUrl(ConfigListName).AddItem();
                    if (Id.HasValue)
                        item[ParentFieldName] = new SPFieldLookupValue(Id.Value, Id.Value.ToString());
                    item[KeyFieldName] = key;
                }
                item[ValueFieldName] = value;
                item.Update();
            }
        }

        protected void CheckId()
        {
            if (!Id.HasValue)
                throw new Exception("Не установлено значение Id в объекте SettingsProvider");
        }

        public string Value
        {
            get
            {
                CheckId();
                return Web.GetListByUrl(ConfigListName).GetItemById(Id.Value).GetFieldValue<string>(ValueFieldName);
            }
            set
            {
                CheckId();
                SPListItem item = Web.GetListByUrl(ConfigListName).GetItemById(Id.Value);
                item[ValueFieldName] = value;
                item.Update();
            }
        }


        public string Key
        {
            get
            {
                CheckId();
                return Web.GetListByUrl(ConfigListName).GetItemById(Id.Value).GetFieldValue<string>(KeyFieldName);
            }
            set
            {
                CheckId();
                SPListItem item = Web.GetListByUrl(ConfigListName).GetItemById(Id.Value);
                item[KeyFieldName] = value;
                item.Update();
            }
        }
    }
}
