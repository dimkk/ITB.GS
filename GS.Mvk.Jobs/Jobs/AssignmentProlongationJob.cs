using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GS.Mvk.Jobs
{
    public class AssignmentProlongationJob : SPJobDefinition
    {
        public static readonly string JobName = "ГС.МВК: Продления поручений";
        
        public static readonly string AssignmentListName = "AssignmentMVKList";
        public static readonly string AssignmentProlongationCountFieldName = "AssignmentProlongationCountMVK";
        public static readonly string AssignmentFactDateFieldName = "AssignmentFactDateMVK";

        public static readonly string ReportListName = "ReportMVKList";
        public static readonly string ReportDecisionFieldName = "ReportDecisionMVK";
        public static readonly string ReportAssignmentFieldName = "ReportAssignmentMVK";

        public AssignmentProlongationJob()
            : base()
        {
            Title = JobName;
        }

        public AssignmentProlongationJob(SPService service)
            : base(JobName, service, null, SPJobLockType.ContentDatabase)
        {
            Title = JobName;
        }

        public AssignmentProlongationJob(SPWebApplication webapp)
            : base(JobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            Title = JobName;
        }

        /// <summary>
        /// Динамический запрос для выборки всех поручений, входящих в список
        /// </summary>
        /// <param name="dict">
        /// Список поручений
        /// </param>
        /// <returns>
        /// Строка CAML запроса
        /// </returns>
        private string buildQuery(Dictionary<string, int> dict)
        {
            if (dict.Count == 0) return String.Empty;

            /// building the query
            string res = @"<Where><In><FieldRef Name='ID'/><Values>";
            foreach (KeyValuePair<string, int> item in dict)
            {
                res += @"<Value Type='Integer'>" + item.Key + @"</Value>";
            }
            res += @"</Values></In></Where>";

            return res;
        }

        /// <summary>
        /// Получение таблицы, содержащей количество отчетов по заданному критерию для каждого поручения.
        /// В таблицу включены только те поручения, у которых количество отчетов не нулевое (см. updateZeroCounters).
        /// </summary>
        /// <param name="list">Список отчетов</param>
        /// <returns>Словарь, в котором ключом является идентификатор поручения, а значением - количество отчетов</returns>
        private Dictionary<string, int> getCurrentReportsState(SPList list)
        {
            SPListItemCollection items = list.GetItems(new SPQuery()
            {
                Query = string.Format(@"<Where>
                            <And>
                                <Eq>
                                    <FieldRef Name='{0}'/>
                                    <Value Type='Choice'>Перенести срок</Value>
                                </Eq>
                                <IsNotNull>
                                    <FieldRef Name='{1}'/>
                                </IsNotNull>
                            </And>
                          </Where>", ReportDecisionFieldName, ReportAssignmentFieldName)
            });

            var state = new Dictionary<string, int>();
            foreach (SPListItem item in items)
            {
                string key = new SPFieldLookupValue(item[ReportAssignmentFieldName].ToString()).LookupId.ToString();
                if (!state.ContainsKey(key))
                {
                    state[key] = 0;
                }

                state[key] += 1;
            }

            return state;
        }

        /// <summary>
        /// Обновление счетчика отчетов в поручениях в соответствии со значениями словаря
        /// </summary>
        /// <param name="state">Словарь</param>
        /// <param name="list">Список поручений</param>
        private void updateAssignmentList(Dictionary<string, int> state, SPList list)
        {
            // если нет отчетов обновлять нечего
            if (state.Count == 0) return;

            SPListItemCollection assignments = list.GetItems(new SPQuery()
            {
                Query = buildQuery(state)
            });

            foreach (SPListItem item in assignments)
            {
                if (Convert.ToInt32(item[AssignmentProlongationCountFieldName]) == state[item["ID"].ToString()]) continue;

                item[AssignmentProlongationCountFieldName] = state[item["ID"].ToString()];
                item.Update();
            }
        }

        /// <summary>
        /// Обновление счетчиков поручений до нуля. Алгоритм предполагает, что все поручения, которые имели 
        /// счетчик отчетов, отличный от нуля, не попали в словарь только в том случае, если отчетов больше нет.
        /// Алгоритм не обновляет поручения, жизненный цикл которых завершен, т.е. установлено значение
        /// поля "Фактическая дата выполнения"
        /// </summary>
        /// <param name="state">Словарь</param>
        /// <param name="list">Список поручений</param>
        private void updateZeroCounters(Dictionary<string, int> state, SPList list)
        {
            SPListItemCollection assignments = list.GetItems(new SPQuery()
            {
                Query = string.Format(@"<Where>
                            <And>
                                <Or>
                                    <Neq>
                                        <FieldRef Name='{0}'/>
                                        <Value Type='Integer'>0</Value>
                                    </Neq>
                                    <IsNull>
                                        <FieldRef Name='{0}'/>
                                    </IsNull>
                                </Or>
                                <IsNull>
                                    <FieldRef Name='{1}'/>
                                </IsNull>
                            </And>
                          </Where>", AssignmentProlongationCountFieldName, AssignmentFactDateFieldName)
            });

            foreach (SPListItem item in assignments)
            {
                if (!state.ContainsKey(item["ID"].ToString()))
                {
                    item[AssignmentProlongationCountFieldName] = 0;
                    item.Update();
                }
            }
        }

        public override void Execute(Guid targetInstanceId)
        {
            Log.Info("Начало работы {0}", JobName);
            var exceptions = new StringBuilder();

            foreach (SPSite site in WebApplication.Sites)
            {
                SPList assignmentList = site.RootWeb.TryGetListByUrl(AssignmentListName);
                if (assignmentList == null)
                {
                    Log.Info("Список {0} не найден на сайте {1}", AssignmentListName, site.RootWeb.Url);
                    continue;
                }

                SPList reportList = site.RootWeb.TryGetListByUrl(ReportListName);
                if (reportList == null)
                {
                    Log.Info("Список {0} не найден на сайте {1}", ReportListName, site.RootWeb.Url);
                    continue;
                }

                try
                {
                    Log.Info("Получение количества отчетов по поручению");
                    var res = getCurrentReportsState(reportList);
                    Log.Info("Начало обработки поручений, общее количество: {0}", res.Count);
                    updateAssignmentList(res, assignmentList);
                    updateZeroCounters(res, assignmentList);
                }
                catch (Exception ex)
                {
                    exceptions.AppendLine(Log.Unexpected(ex, "Ошибка обновления поручений ({0}) на сайте {1}", AssignmentListName, site.RootWeb.Url));
                }
            }
            Log.Info("Конец работы {0}", JobName);

            if (exceptions.Length > 0)
                throw new Exception(exceptions.ToString());
        }
    }
}
