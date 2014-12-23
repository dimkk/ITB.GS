using System;
using System.Linq;
using CamlexNET;
using ITB.SP.Tools;
using Microsoft.SharePoint;

namespace GS.Common.BL
{
    public enum StatusEnum
    {
        IgReceivedNotify,           //Получено уведомление из САМРТ
        IgGetDataError,             //Ошибка получения данных
        IgAdded,                    //Получено из САМРТ (добавлено в плановые вопросы)
        IgAddedSendError,           //Ошибка отправки данных (добавлено в плановые вопросы)
        IgAddedSent,                //Отправлено в САМРТ (добавлено в плановые вопросы)
        MvkIncluded,                //Включено в повестку МВК
        GsIncluded,                 //Включено в повестку ГС
        IgMvkIncludedSendError,     //Ошибка отправки данных (включено в повестку МВК)
        IgMvkIncludedSent,          //Отправлено в САМРТ (включено в повестку МВК)
        IgGsIncludedSendError,      //Ошибка отправки данных (включено в повестку ГС)
        IgGsIncludedSent,           //Отправлено в САМРТ (включено в повестку ГС)
        MvkConsidered,              //Рассмотрено на МВК
        GsConsidered,               //Рассмотрено на ГС
        IgMvkConsideredSendError,   //Ошибка отправки данных (рассмотрено на МВК)
        IgMvkConsideredSent,        //Отправлено в САМРТ (рассмотрено на МВК)
        IgGsConsideredSendError,    //Ошибка отправки данных (рассмотрено на ГС)
        IgGsConsideredSent          //Отправлено в САМРТ (рассмотрено на ГС)
    }

    public class Status
    {
        protected static readonly string StatusListName = "Status";
        protected static readonly string StatusKeyFieldName = "StatusKey";

        public static StatusEnum GetById(SPWeb web, int statusId)
        {
            return web.GetListByUrl(StatusListName)
                .GetItemById(statusId)
                .GetFieldValue<string>(StatusKeyFieldName)
                .EnumParse<StatusEnum>();
        }

        public static int GetIdByStatus(SPWeb web, StatusEnum status)
        {
            SPQuery query = Camlex.Query().Where(x => x[StatusKeyFieldName] == (DataTypes.Text) status.ToString()).ToSPQuery();
            return web.GetListItems(StatusListName, query).Single().ID;
        }
    }
}
