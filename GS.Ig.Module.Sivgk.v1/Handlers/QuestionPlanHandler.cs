using ITB.Ig.Interface;
using ITB.Ig.Module.Sivgk.v1.Handlers;
using ITB.Ig.Module.Sivgk.v1.Interfaces;
using Ninject;
using System.Collections.Generic;

namespace GS.Ig.Module.Sivgk.v1.Handlers
{
    /// <summary>
    /// Реализация обработчика типа Плановые вопросы на сайте ГС
    /// </summary>    
    public class QuestionPlanHandler : QuestionHandler
    {
        public QuestionPlanHandler(ILogger logger, ISettings settings, IHistoryRepository historyRepository, IMessageRepository messageRepository, [Named("Plan")] IQuestionRepository questionRepository)
            : base(logger, settings, historyRepository, messageRepository, questionRepository)
        {
        }

        public override bool IsDictionary
        {
            get { return false; }
        }

        public override IEnumerable<string> HandleDataTypes
        {
            get
            {
                return new[]
                {
                    IgEntityType.IssueRg.ToString(),
                    IgEntityType.IssueAk.ToString(),
                    IgEntityType.DocumentTp.ToString()
                };
            }
        }
    }
}