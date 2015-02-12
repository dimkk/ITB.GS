using ITB.Ig.Interface;
using ITB.Ig.Module.Sivgk.v1;
using ITB.Ig.Module.Sivgk.v1.Handlers;
using ITB.Ig.Module.Sivgk.v1.Interfaces;
using ITB.Ig.Module.Sivgk.v1.Models;
using Microsoft.SharePoint;
using System;
using System.ServiceModel;
using NotificationClient = ITB.Ig.Module.Sivgk.v1.NotificationService;

namespace GS.Ig.Module.Sivgk.v1.Senders
{
    public class IgSender : IIgSender
    {
        protected readonly ISettings Settings;
        protected readonly IHistoryRepository HistoryRepository;
        protected readonly IMessageRepository MessageRepository;

        public IgSender(ISettings settings, IHistoryRepository historyRepository, IMessageRepository messageRepository)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (historyRepository == null)
                throw new ArgumentNullException("historyRepository");
            if (messageRepository == null)
                throw new ArgumentNullException("messageRepository");

            Settings = settings;
            HistoryRepository = historyRepository;
            MessageRepository = messageRepository;
        }

        /// <summary>
        /// вызывает метод СИВГК HasNewData, передавая ему GUID измененного элемента , 
        /// вызов метода протоколируется в таблице IgMessage .
        /// </summary>
        /// <param name="target"></param>
        /// <param name="entityType"></param>
        public void SendEntity(SPListItem target, IgEntityType entityType)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var history = new HistoryModel(Settings, target, entityType);
            HistoryRepository.Save(history);

            BaseHandler.RequestService<NotificationClient.hasNewDataResponse>(Settings, HistoryRepository, MessageRepository, history, EMessageType.HasNewData,
                (message) =>
                {
                    using (var client = new NotificationClient.NotificationLinkPortTypeClient())
                    {
                        var request = new NotificationClient.hasNewDataRequest()
                        {
                            messageInfo = new NotificationClient.messageInfo()
                            {
                                from = new NotificationClient.informationSystemData() { name = message.SystemSender },
                                to = new NotificationClient.informationSystemData() { name = message.SystemReciever }
                            },
                            revision = new NotificationClient.revision()
                            {
                                timestampSpecified = false,
                                version = history.QuestionID.ToString()
                            },
                            type = history.EntityType.ToString()
                        };

                        client.Endpoint.Address = new EndpointAddress(ServiceUrlManager.GetNotificationUrl(Settings, message));

                        NotificationClient.hasNewDataResponse result = client.hasNewData(request);

                        if (result == null || !result.succeeded)
                            throw new UnSuccessfulRequestException(string.Format("Сообщение не доставлено. Веб-сервис по адресу {0} не подтвердил доставку сообщения", client.Endpoint.Address));

                        return result;
                    }
                });
        }
    }
}