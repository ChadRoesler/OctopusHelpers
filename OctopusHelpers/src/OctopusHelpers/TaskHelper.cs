using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Task Objects.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Returns only Errors from a task.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather.</param>
        /// <returns>A string of Errors</returns>
        public static string GetErrors(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = GetActivityElementList(octRepository, task);
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

        /// <summary>
        /// Returns only warnings from a task.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather.</param>
        /// <returns>A string of warnings</returns>
        public static string GetWarnings(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = GetActivityElementList(octRepository, task);
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

        /// <summary>
        /// Returns errors and warnings from a task
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather.</param>
        /// <returns>A string of warnings and errors</returns>
        public static string GetErrorsAndWarnings(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = GetActivityElementList(octRepository, task);
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

        /// <summary>
        /// Returns the full log of a task
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather.</param>
        /// <returns>String of the whole log</returns>
        public static string GetFullLog(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = GetActivityElementList(octRepository, task);
            var output = string.Empty;
            foreach (var activityStep in activitySteps)
            {
                output += ActivityElementHelper.GetLogInfo(activityStep, 0, null) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        /// <summary>
        /// Gathers task Step Count Total.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather.</param>
        /// <returns>Number of steps</returns>
        public static int GetTaskStepCount(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = octRepository.Tasks.GetDetails(task).ActivityLogs.FirstOrDefault().Children;
            var activityStepCount = activitySteps.Count();
            return activityStepCount;
        }

        /// <summary>
        /// Gathers task from Id passed.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="taskId">Task Id to gather.</param>
        /// <returns>TaskResource</returns>
        public static TaskResource GetTaskFromId(OctopusRepository octRepository, string taskId)
        {
            var numberOnly = new int();
            if (int.TryParse(taskId, out numberOnly))
            {
                return octRepository.Tasks.Get(string.Format(ResourceStrings.TaskIdFormat, taskId));
            }
            else
            {
                return octRepository.Tasks.Get(taskId);
            }
        }

        /// <summary>
        /// Gathers task step names.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to Gather from.</param>
        /// <returns>Enumerable of Task Step Names as strings</returns>
        public static IEnumerable<string> GetTaskStepNames(OctopusRepository octRepository, TaskResource task)
        {
            var activitySteps = GetActivityElementList(octRepository, task);
            return activitySteps.Select(x => x.Name);
        }

        /// <summary>
        /// Gathers the Activity Elements of a task.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to Gather from.</param>
        /// <returns>ActivitityElents of a task</returns>
        public static ActivityElement[] GetActivityElementList(OctopusRepository octRepository, TaskResource task)
        {
           return octRepository.Tasks.GetDetails(task).ActivityLogs.FirstOrDefault().Children;
        }
    }
}
