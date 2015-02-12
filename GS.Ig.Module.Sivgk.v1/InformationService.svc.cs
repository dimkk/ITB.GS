using ITB.Ig.Module.Sivgk.v1.Interfaces;
using ITB.Ig.Module.Sivgk.v1.Services;
using System.ServiceModel;

namespace GS.Ig.Module.Sivgk.v1.Services
{
    [ServiceBehavior]
    public class InformationService : IInformationService
    {
        protected readonly IHandlerManager HandlerManager;

        public InformationService(IHandlerManager handlerManager)
        {
            HandlerManager = handlerManager;
        }

        [OperationBehavior]
        public getResponse1 get(getRequest1 request)
        {
            return HandlerManager.GetHandler(request).get(request);
        }
    }
}
