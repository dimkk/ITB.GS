using ITB.SP.Tools;
using Microsoft.SharePoint;
using Ninject;
using System;
using System.Collections.Generic;
using System.Reflection;
using SAMRT.Ig.Interface;
using ISettingsProvider = GS.Ig.Core.Interfaces.ISettingsProvider;

namespace GS.Ig.Core
{
    public class IgSenderManager
    {
        private static IEnumerable<IIgSender> IgSenders;

        public static void SendEntity(SPListItem sendItem, IgEntityType entityType)
        {
            if (IgSenders == null)
                IgSenders = InitializeSenders(sendItem);

            foreach (IIgSender sender in IgSenders)
                sender.SendEntity(sendItem, entityType);
        }

        /// <summary>
        /// Инициализирует коллекцию IgSenders при первом запуске и апоминает реализацию SPWeb
        /// </summary>
        /// <param name="sendItem"></param>
        /// <returns></returns>
        private static IEnumerable<IIgSender> InitializeSenders(SPListItem sendItem)
        {
            var settingsProvider = new SettingsProvider(sendItem.Web);

            var kernel = new StandardKernel();
            kernel.Bind<SPWeb>().ToConstant(sendItem.Web);

            foreach (ISettingsProvider moduleProvider in settingsProvider.GetAll())
                try
                {
                    var assembly = Assembly.Load(moduleProvider.Value);
                    kernel.Load(assembly);
                }
                catch (Exception ex)
                {
                    Log.Unexpected(ex, "Ошибка загрузки модуля интеграции (Key = '{0}', Value = '{1}')", moduleProvider.Key, moduleProvider.Value);
                }

            var igSenders = kernel.GetAll<IIgSender>();

            return igSenders;
        }
    }
}
