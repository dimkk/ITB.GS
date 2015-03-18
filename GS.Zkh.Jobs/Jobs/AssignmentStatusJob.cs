using ITB.SP.Tools;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Text;

namespace GS.Zkh.Jobs
{
    class AssignmentStatusJob : SPJobDefinition
    {
        public static readonly string JobName = "ГС.ЖКХ: Статусы поручений";

        public static readonly string AssignmentListName = "AssignmentZkhList";
        public static readonly string AssignmentStatusFieldName = "AssignmentStatusZkh";
        public static readonly string AssignmentPlanDateFieldName = "AssignmentPlanDateZkh";
        public static readonly string AssignmentFactDateFieldName = "AssignmentFactDateZkh";

        public AssignmentStatusJob()
            : base()
        {
            Title = JobName;
        }

        public AssignmentStatusJob(SPService service)
            : base(JobName, service, null, SPJobLockType.ContentDatabase)
        {
            Title = JobName;
        }

        public AssignmentStatusJob(SPWebApplication webapp)
            : base(JobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            Title = JobName;
        }

        /// <summary>
        /// Обновление статусов поручений
        /// </summary>
        /// <param name="list">Список поручений</param>
        protected SPListItemCollection GetAssignmentsToUpdate(SPList list)
        {
            return list.GetItems(new SPQuery()
            {
                Query = string.Format(@"<Where>
	                        <And>
		                        <And>
			                        <IsNotNull>
				                        <FieldRef Name='{0}' />
			                        </IsNotNull>
			                        <Lt>
				                        <FieldRef Name='{0}' />
				                        <Value Type='DateTime'>
					                        <Today />
				                        </Value>
			                        </Lt>
		                        </And>
		                        <And>
			                        <IsNull>
				                        <FieldRef Name='{1}' />
			                        </IsNull>
			                        <Eq>
				                        <FieldRef Name='{2}' />
				                        <Value Type='Choice'>На исполнении</Value>
			                        </Eq>
		                        </And>
	                        </And>
                        </Where>", AssignmentPlanDateFieldName, AssignmentFactDateFieldName, AssignmentStatusFieldName)
            });
        }

        public override void Execute(Guid targetInstanceId)
        {
            Log.Info("Начало работы {0}", JobName);
            var exceptions = new StringBuilder();

            foreach (SPSite site in WebApplication.Sites)
            {
                Log.Info("Начало обработки сайта {0}", site.RootWeb.Url);
                
                SPList assignmentList = site.RootWeb.TryGetListByUrl(AssignmentListName);
                if (assignmentList == null)
                {
                    Log.Info("Список {0} не найден на сайте {1}", AssignmentListName, site.RootWeb.Url);
                    continue;
                }

                SPListItemCollection assignments = GetAssignmentsToUpdate(assignmentList);
                Log.Info("Начало обработки списка {0}, количество элементов: {1}", AssignmentListName, assignments.Count);
                foreach (SPListItem item in assignments)
                {
                    Log.Info("Начало обработки элемента (ID = {0})", item.ID);
                    item[AssignmentStatusFieldName] = "Срок истек";
                    try
                    {
                        item.Update();
                    }
                    catch (Exception ex)
                    {
                        exceptions.AppendLine(Log.Unexpected(ex, "Ошибка обновления статуса поручения (ID = {0}) списка {1} на сайте {2}", item.ID, AssignmentListName, site.RootWeb.Url));
                    }
                }
            }

            Log.Info("Конец работы {0}", JobName);

            if (exceptions.Length > 0)
                throw new Exception(exceptions.ToString());
        }
    }
}
