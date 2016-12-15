using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Enums;
using OctopusHelpers.Constants;

namespace OctopusHelpers.Models
{
    /// <summary>
    /// Object Built for managing Deployment Tasks.
    /// </summary>
    public class OctopusDeploymentTaskManager
    {
        private OctopusRepository octRepositoryToManage;
        private DeploymentResource deploymentToManage;
        private TaskResource taskToManage;
        private InterruptionResource currentInterruptionToProcess;
        private InterruptionResource previousInterruptionToProcess;
        private ActivityElement[] activitySteps = new ActivityElement[0];
        private Dictionary<string, string> printedLog = new Dictionary<string, string>();
        private bool CancellationSent = false;
        private bool CancellationRequested = false;

        /// <summary>
        /// Allows for an empty pull
        /// </summary>
        public OctopusDeploymentTaskManager()
        {

        }

        /// <summary>
        /// A simple and efficiant way to manage DeploymentTasks
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="deployment"></param>
        public OctopusDeploymentTaskManager(OctopusRepository octRepository, DeploymentResource deployment)
        {
            octRepositoryToManage = octRepository;
            deploymentToManage = deployment;
        }

        /// <summary>
        /// Start the Deploy
        /// </summary>
        public void StartDeploy()
        {
            if (taskToManage == null)
            {
                deploymentToManage = octRepositoryToManage.Deployments.Create(deploymentToManage);
                taskToManage = octRepositoryToManage.Tasks.Get(deploymentToManage.TaskId);
            }
            else
            {
                throw new Exception(string.Format(ErrorStrings.DeploymentAlreadyStarted, deploymentToManage.Id));
            }
        }

        /// <summary>
        /// A call to Cancel that can be managed from other areas;
        /// </summary>
        public void RequestCancellation()
        {
            CancellationRequested = true;
        }

        /// <summary>
        /// Cancel the Deploy.
        /// </summary>
        public void CancelDeploy()
        {
            if (taskToManage != null)
            {
                UpdateTask();
                if (Status == TaskManagerStatus.Executing || Status == TaskManagerStatus.Interrupted || Status == TaskManagerStatus.Queued)
                {
                    octRepositoryToManage.Tasks.Cancel(taskToManage);
                    CancellationSent = true;
                }
                else
                {
                    throw new Exception(string.Format(ErrorStrings.DeploymentNotInCancellableState, deploymentToManage.Id, Enum.GetName(typeof(TaskManagerStatus), Status)));
                }
            }
            else
            {
                throw new Exception(string.Format(ErrorStrings.DeploymentNotStarted, deploymentToManage.Id));
            }
        }

        /// <summary>
        /// Responds to the current pending interruption.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="note"></param>
        public void RespondToInterruption(InterruptionResponse response, string note)
        {
            UpdateInterruption();
            InterruptionHelper.InterruptionReponse(octRepositoryToManage, currentInterruptionToProcess, response, note);
            UpdatePreviousInterruption();
        }

        /// <summary>
        /// Refresh the Task for reporting.
        /// </summary>
        private void UpdateTask()
        {
            taskToManage = octRepositoryToManage.Tasks.Get(taskToManage.Id);
        }

        /// <summary>
        /// Refreshes the current pending interruption, if one exists.
        /// </summary>
        private void UpdateInterruption()
        {
            var updatedInterruption = InterruptionHelper.GetPendingInterruption(octRepositoryToManage, taskToManage);
            if (updatedInterruption != null && !updatedInterruption.Equals(currentInterruptionToProcess))
            {
                currentInterruptionToProcess = updatedInterruption;
            }
        }

        /// <summary>
        /// Refreshes the previous interruption, used for reporting on the guided interruption.
        /// </summary>
        private void UpdatePreviousInterruption()
        {
            previousInterruptionToProcess = octRepositoryToManage.Interruptions.Get(currentInterruptionToProcess.Id);
            while (previousInterruptionToProcess.IsPending || previousInterruptionToProcess.ResponsibleUserId == null || previousInterruptionToProcess.Form == null || previousInterruptionToProcess.Form.Values[ResourceStrings.InterruptionGuidanceKey] == null)
            {
                previousInterruptionToProcess = octRepositoryToManage.Interruptions.Get(currentInterruptionToProcess.Id);
            }
        }

        /// <summary>
        /// Refreshes the current list of activity.
        /// </summary>
        private void UpdateActivity()
        {
            UpdateTask();
            var baseActivityLog = octRepositoryToManage.Tasks.GetDetails(taskToManage).ActivityLogs.FirstOrDefault();
            while (baseActivityLog == null || baseActivityLog.Children == null || baseActivityLog.Children.Count() == 0)
            {
                var taskDetails = octRepositoryToManage.Tasks.GetDetails(taskToManage);
                baseActivityLog = taskDetails.ActivityLogs.FirstOrDefault();
            }
            activitySteps = octRepositoryToManage.Tasks.GetDetails(taskToManage).ActivityLogs.FirstOrDefault().Children;
        }

        /// <summary>
        /// Gathers all failures/error messages.
        /// </summary>
        /// <param name="activityElementToProcess"></param>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        private string GetFailures(ActivityElement activityElementToProcess, int tabIndex)
        {
            var output = string.Empty;
            var tabCount = new string(' ', tabIndex * 5);
            if (activityElementToProcess.Status == ActivityStatus.Failed || activityElementToProcess.Status == ActivityStatus.SuccessWithWarning || activityElementToProcess.Status == ActivityStatus.Running)
            {
                output += activityElementToProcess.Name;
                foreach (var activityLogElement in activityElementToProcess.LogElements)
                {
                    output += (string.Format(ResourceStrings.ErrorPrinting, tabCount, activityLogElement.MessageText.Replace(ResourceStrings.Return, string.Format(ResourceStrings.ErrorPrinting, ResourceStrings.Return, tabCount))));
                }
                foreach (var activityElement in activityElementToProcess.Children)
                {
                    output += GetFailures(activityElement, tabIndex + 1);
                }
            }
            return output.Trim();
        }

        /// <summary>
        /// Returns the important information, step and sub step info, about the Errors and Warnings that occured.
        /// </summary>
        /// <returns></returns>
        public string GetErrorsAndWarnings()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.Failed || a.Status == ActivityStatus.SuccessWithWarning))
            {
                output += GetFailures(activityStep, 0);
            }
            return output.Trim();
        }

        /// <summary>
        /// Returns the important information, step and sub step info, about the Warnings that occured.
        /// </summary>
        /// <returns></returns>
        public string GetWarnings()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.SuccessWithWarning))
            {
                output += GetFailures(activityStep, 0);
            }
            return output.Trim();
        }

        /// <summary>
        /// Returns the important information, step and sub step info, about the Errors that occured.
        /// </summary>
        /// <returns></returns>
        public string GetErrors()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.Failed))
            {
                output += GetFailures(activityStep, 0);
            }
            return output.Trim();
        }

        /// <summary>
        /// Gets the User that canceled the Deployment
        /// </summary>
        /// <returns></returns>
        public UserResource GetCancellingUser()
        {
            if(taskToManage.State == TaskState.Canceled || taskToManage.State == TaskState.Cancelling)
            {
                return UserHelper.GetUserFromUserName(octRepositoryToManage, taskToManage.LastModifiedBy);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the output for logging, only gathers non-previously reported info.
        /// </summary>
        /// <returns></returns>
        public string GetLog()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => !printedLog.Any(p => p.Key.Equals(a.Id, StringComparison.OrdinalIgnoreCase))))
            {
                if (activityStep.Status != ActivityStatus.Pending)
                {
                    printedLog.Add(activityStep.Id, activityStep.Name);
                    output += activityStep.Name;
                }
            }
            return output.Trim();
        }

        /// <summary>
        /// Gets the current Step count.
        /// </summary>
        /// <returns></returns>
        public int GetStepCount()
        {
            return activitySteps.Count();
        }

        /// <summary>
        /// Gets the currently completed Step count.
        /// </summary>
        /// <returns></returns>
        public int GetStepCompletedCount()
        {
            return printedLog.Count();
        }

        /// <summary>
        /// Gets the current count of deployments ahead of the current one.
        /// </summary>
        /// <returns></returns>
        public int GetQueuedDeploymentCount()
        {
            return DeploymentHelper.GetQueuedDeployments(octRepositoryToManage, deploymentToManage).Count();
        }
        
        /// <summary>
        /// Get the Step the the failure occured on for Interruptions.
        /// </summary>
        /// <returns></returns>
        public string GetInterruptedStepInfo()
        {
            UpdateActivity();
            var output = string.Empty;
            output += activitySteps.Where(a => a.Status == ActivityStatus.Running).FirstOrDefault().Name;
            output += GetFailures(activitySteps.Where(a => a.Status == ActivityStatus.Running).FirstOrDefault().Children.LastOrDefault(), 0);
            return output.Trim();
        }

        /// <summary>
        /// Returns the Last Step Executed
        /// </summary>
        /// <returns></returns>
        public string GetLastStepExecuted()
        {
            if (printedLog.Count() > 0)
            {
                return printedLog.LastOrDefault().Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Note of the Guided Interruption
        /// </summary>
        /// <returns></returns>
        public string GetManagedInterruptionNote()
        {
            UpdatePreviousInterruption();
            return previousInterruptionToProcess.Form.Values[ResourceStrings.InterruptionNoteKey];
        }

        /// <summary>
        /// Gets the Guideance Taken of the Guided Interruption
        /// </summary>
        /// <returns></returns>
        public string GetManagedInterruptionGuidence()
        {
            UpdatePreviousInterruption();
            return previousInterruptionToProcess.Form.Values[ResourceStrings.InterruptionGuidanceKey];
        }

        /// <summary>
        /// Gets the Guiding User of the Guided Interruption
        /// </summary>
        /// <returns></returns>
        public UserResource GetManagedInterruptionResponsibleUser()
        {
            UpdatePreviousInterruption();
            return UserHelper.GetUserFromUserId(octRepositoryToManage, previousInterruptionToProcess.ResponsibleUserId);
        }

        public string GetDeploymentLink()
        {
            if (taskToManage != null)
            {
                return DeploymentHelper.GetDeploymentLinkForWeb(deploymentToManage);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the current Status of the Deployment
        /// </summary>
        /// <returns></returns>
        public TaskManagerStatus Status
        {
            get
            {
                var currentState = TaskManagerStatus.NotStarted;
                if (taskToManage != null)
                {
                    UpdateTask();
                    UpdateInterruption();
                    if (taskToManage.HasPendingInterruptions && taskToManage.State == TaskState.Executing)
                    {
                        currentState = TaskManagerStatus.Interrupted;
                    }
                    else if (taskToManage.IsCompleted && taskToManage.State != TaskState.Canceled)
                    {
                        currentState = TaskManagerStatus.Completed;
                    }
                    else
                    {
                        switch (taskToManage.State)
                        {
                            case TaskState.Canceled:
                                currentState = TaskManagerStatus.Canceled;
                                break;
                            case TaskState.Queued:
                                currentState = TaskManagerStatus.Queued;
                                break;
                            case TaskState.Executing:
                                currentState = TaskManagerStatus.Executing;
                                break;
                            case TaskState.Cancelling:
                                currentState = TaskManagerStatus.Canceling;
                                break;
                        }
                    }
                }
                return currentState;
            }
        }

        /// <summary>
        /// Gets the status of the Cancellation
        /// </summary>
        public CancellationStatus CancellationStatus
        {
            get
            {
                var cancelationState = CancellationStatus.None;
                if (CancellationRequested)
                {
                    cancelationState = CancellationStatus.CancellationRequested;
                }
                if (CancellationSent)
                {
                    cancelationState = CancellationStatus.CancellationSent;
                }
                if (Status == TaskManagerStatus.Canceled)
                {
                    cancelationState = CancellationStatus.Canceled;
                }
                return cancelationState;
            }
        }

        /// <summary>
        /// Gets the Error Status of the Task
        /// </summary>
        public ErrorStatus ErrorStatus
        {
            get
            {
                UpdateActivity();
                var errorState = ErrorStatus.Success;
                if(activitySteps.Count(a => a.Status == ActivityStatus.SuccessWithWarning) > 0)
                {
                    errorState = ErrorStatus.Warnings;
                }
                if(activitySteps.Count(a => a.Status == ActivityStatus.Failed) > 0)
                {
                    errorState = ErrorStatus.Error;
                }
                return errorState;
            }
        }
    }
}
