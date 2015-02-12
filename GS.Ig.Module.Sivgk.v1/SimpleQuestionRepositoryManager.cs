using ITB.Ig.Module.Sivgk.v1.Interfaces;
using ITB.Ig.Module.Sivgk.v1.Models;
using Ninject;

namespace GS.Ig.Module.Sivgk.v1
{
    /// <summary>
    /// Простой менеджер работы с вопросами
    /// </summary>
    public class SimpleQuestionRepositoryManager : IQuestionRepositoryManager
    {
        private IQuestionRepository _repositiry;

        public SimpleQuestionRepositoryManager(
            [Named("Plan")]
                IQuestionRepository repositiry)
        {
            _repositiry = repositiry;
        }

        public IQuestionRepository GetRepository(ESourceEntityType sourceEntityType)
        {
            return _repositiry;
        }
    }
}