using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Runtime.InteropServices;

namespace GS.Zkh.Jobs.Features
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("5BBF7784-DA81-40DA-B13C-60B3FF417AB3")]
    public class GSZkhEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                SPSite site = properties.Feature.Parent as SPSite;
                DeleteExistingJob(AssignmentProlongationJob.JobName, parentWebApp);
                DeleteExistingJob(AssignmentStatusJob.JobName, parentWebApp);
                CreateJobs(parentWebApp);
            });
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                DeleteExistingJob(AssignmentProlongationJob.JobName, parentWebApp);
                DeleteExistingJob(AssignmentStatusJob.JobName, parentWebApp);
            });
        }

        private void CreateJobs(SPWebApplication site)
        {
            AssignmentProlongationJob job = new AssignmentProlongationJob(site);
            SPMinuteSchedule schedule = new SPMinuteSchedule();
            schedule.BeginSecond = 0;
            schedule.EndSecond = 59;
            schedule.Interval = 1;
            job.Schedule = schedule;
            job.Update();

            AssignmentStatusJob jobExpStatuses = new AssignmentStatusJob(site);
            SPDailySchedule dailyschedule = new SPDailySchedule();
            dailyschedule.BeginHour = 6;
            dailyschedule.BeginMinute = 0;
            dailyschedule.BeginSecond = 0;
            dailyschedule.EndSecond = 15;
            dailyschedule.EndMinute = 0;
            dailyschedule.EndHour = 6;
            jobExpStatuses.Schedule = dailyschedule;
            jobExpStatuses.Update();
        }

        public bool DeleteExistingJob(string jobName, SPWebApplication site)
        {
            bool jobDeleted = false;
            try
            {
                foreach (SPJobDefinition job in site.JobDefinitions)
                {
                    if (job.Name == jobName)
                    {
                        job.Delete();
                        jobDeleted = true;
                    }
                }
            }
            catch (Exception)
            {
                return jobDeleted;
            }
            return jobDeleted;
        }
    }
}
