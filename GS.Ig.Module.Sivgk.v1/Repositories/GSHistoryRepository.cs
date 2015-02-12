using ITB.Ig.Module.Sivgk.v1.Models;
using ITB.Ig.Module.Sivgk.v1.Repositories;
using Microsoft.SharePoint;

namespace GS.Ig.Module.Sivgk.v1.Repositories
{
    /// <summary>
    /// Репозитарий для работы с историей в ГС, отличается тем что ид вопроса вне зависимости от типа 
    /// запихивает в "Плановый вопрос"
    /// </summary>
    public class GSHistoryRepository : HistoryRepository
    {
        public GSHistoryRepository(SPWeb oWeb) : base(oWeb)
        {
        }

        protected override void SetQuestionID(HistoryModel model, SPListItem item)
        {
            item["Плановый вопрос"] = model.QuestionID;            
        }
    }
}