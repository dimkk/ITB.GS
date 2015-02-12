using GS.Ig.Module.Sivgk.v1.Services;
using ITB.Ig.Interface;
using Ninject;
using Ninject.Extensions.Wcf;
using Ninject.Web.Common;
using System;
using System.ServiceModel.Activation;
using System.Web.Routing;

namespace GS.Ig.Module.Sivgk.v1
{
    public class Global : NinjectHttpApplication
    {
        private StandardKernel _kernel;

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            _kernel.Get<ILogger>().Error(Server.GetLastError().ToString());
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
        protected override void OnApplicationStarted()
        {
            var factory = new NinjectServiceHostFactory();
            RouteTable.Routes.Add(new ServiceRoute("Notification", factory, typeof(NotificationService)));
            RouteTable.Routes.Add(new ServiceRoute("Information", factory, typeof(InformationService)));
        }

        protected override IKernel CreateKernel()
        {
            _kernel = new StandardKernel(new ServiceModule());
            return _kernel;
        }
    }
}