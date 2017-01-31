using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class TaskHelper
    {
        public static string GetErrors(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = octRepository.Tasks.GetDetails(task).ActivityLogs.FirstOrDefault().Children;
            var output = string.Empty;
            var statusList = new List<ActivityStatus>
            {
                ActivityStatus.Failed
            };
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.Failed))
            {
                output += ActivityElementHelper.GetLogInfo(activityStep, 0, statusList) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        public static string GetWarnings(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = octRepository.Tasks.GetDetails(task).ActivityLogs.FirstOrDefault().Children;
            var output = string.Empty;
            var statusList = new List<ActivityStatus>
            {
                ActivityStatus.SuccessWithWarning
            };
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.SuccessWithWarning))
            {
                output += ActivityElementHelper.GetLogInfo(activityStep, 0, statusList) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        public static string GetErrorsAndWarnings(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = octRepository.Tasks.GetDetails(task).ActivityLogs.FirstOrDefault().Children;
            var output = string.Empty;
            var statusList = new List<ActivityStatus>
            {
                ActivityStatus.Failed,
                ActivityStatus.SuccessWithWarning
            };
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.Failed || a.Status == ActivityStatus.SuccessWithWarning))
            {
                output += ActivityElementHelper.GetLogInfo(activityStep, 0, statusList) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        public static string GetFullLog(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = octRepository.Tasks.GetDetails(task).ActivityLogs.FirstOrDefault().Children;
            var output = string.Empty;
            foreach (var activityStep in activitySteps)
            {
                output += ActivityElementHelper.GetLogInfo(activityStep, 0, null) + ResourceStrings.Return;
            }
            return output.Trim();
        }
    }
}
