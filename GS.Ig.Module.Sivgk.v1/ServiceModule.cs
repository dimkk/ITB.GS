using GS.Ig.Core;
using GS.Ig.Module.Sivgk.v1.Repositories;
using ITB.Ig.Interface;
using ITB.Ig.Module.Sivgk.v1;
using ITB.Ig.Module.Sivgk.v1.NotificationService;
using ITB.Ig.Module.Sivgk.v1.Interfaces;
using ITB.Ig.Module.Sivgk.v1.Repositories;
using Microsoft.SharePoint;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace GS.Ig.Module.Sivgk.v1
{
    public class ServiceModule : NinjectModule 
    {
        public override void Load()
        {
            //биндим логгер
            Kernel.Bind<ILogger>().To<NLogProxy>().InSingletonScope();

            ///если реализации нет - делаем свою
            if (Kernel.TryGet<SPWeb>() == null)
            {
                HttpContext.Current.User = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                Kernel.Bind<SPWeb>()
                    .ToMethod((a) => new SPSite(System.Configuration.ConfigurationManager.AppSettings["SiteUrl"], SPUserToken.SystemAccount).RootWeb)
                    .InSingletonScope();
            }

            var web = Kernel.Get<SPWeb>();
            Kernel.Bind<ISettingsProvider>().To<SettingsProvider>().InSingletonScope();
            Kernel.Bind<ISettings>()
                .ToConstructor(c => new Settings(Kernel.Get<ISettingsProvider>(), "GS.Ig.Module.Sivgk.v1", web.Url))                
                .InSingletonScope();

            //репозитарий для работы со списком муниципальных образований
            Kernel.Bind<IBuildersRepository>().To<BuildersRepository>();
            Kernel.Bind<IObjectRepository>().To<ObjectRepository>();
            Kernel.Bind<IHistoryRepository>().To<GSHistoryRepository>();
            Kernel.Bind<IMessageRepository>().To<MessageRepository>();
            Kernel.Bind<IMunicipalityRepository>() ///.ToConstant(new MunicipalityRepository(site.RootWeb));
                .ToConstructor(i => new MunicipalityRepository(Kernel.Get<SPWeb>()));

            Kernel.Bind<IQuestionRepository>().To<QuestionPlanRepository>().Named("Plan");

            Kernel.Bind<IQuestionRepositoryManager>().To<SimpleQuestionRepositoryManager>();

            //Регистрируем отправителей
            Kernel.Bind(t => t.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IIgSender>()
                .BindAllInterfaces());

            //Регистрируем обработчики типов данных
            Kernel.Bind(t => t.From(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("GS.Ig.") || a.FullName.StartsWith("ITB.Ig.")))
                .SelectAllClasses()
                .InheritedFrom<IDataTypeHandler>()
                .BindAllInterfaces());
          
            //Менеджер обработчиков
            Kernel.Bind<IHandlerManager>().To<HandlerManager>().InSingletonScope();

            if (Kernel.TryGet<NotificationLinkPortType>() == null)
                Kernel.Bind<NotificationLinkPortType>().To<NotificationLinkPortTypeClient>();
        }        
    }
}