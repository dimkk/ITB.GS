using ITB.Ig.Module.Sivgk.v1.Interfaces;
using ITB.Ig.Module.Sivgk.v1.Services;
using System.ServiceModel;

namespace GS.Ig.Module.Sivgk.v1.Services
{
    [ServiceBehavior]
    public class NotificationService : INotificationService
    {
        protected readonly IHandlerManager HandlerManager;

        public NotificationService(IHandlerManager handlerManager)
        {
            HandlerManager = handlerManager;
        }

        [OperationBehavior]
        public hasNewDataResponse1 hasNewData(hasNewDataRequest1 request)
        {
            return HandlerManager.GetHandler(request).hasNewData(request);
        }

        [OperationBehavior]
        public requestDataResponse1 requestData(requestDataRequest1 request)
        {
            return HandlerManager.GetHandler(request).requestData(request);
        }
    }
}
