using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private ActivityElement[] activitySteps = new ActivityElement[0];
        private Dictionary<string, string> printedLog = new Dictionary<string, string>();
        private bool CancellationSent = false;
        private bool CancellationRequested = false;

        /// <summary>
        /// Allows for inspection of the current interruption and intervention.
        /// </summary>
        public InterruptionResource currentInterruptionToProcess;

        /// <summary>
        /// Allows for inspection of the previous interruption and intervention.
        /// </summary>
        public InterruptionResource previousInterruptionToProcess;

        /// <summary>
        /// Allows for inspection of the current interruption and intervention.
        /// </summary>
        public InterruptionResource currentInterventionToProcess;

        /// <summary>
        /// Allows for inspection of the previous interruption and intervention.
        /// </summary>
        public InterruptionResource previousInterventionToProcess;

        /// <summary>
        /// Allows base creation of the object.
        /// </summary>
        public OctopusDeploymentTaskManager()
        {

        }

        /// <summary>
        /// A simple and efficient way to manage DeploymentTasks
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="deployment">The deployment to manage.</param>
        public OctopusDeploymentTaskManager(OctopusRepository octRepository, DeploymentResource deployment)
        {
            octRepositoryToManage = octRepository;
            deploymentToManage = deployment;
            if(!string.IsNullOrWhiteSpace(deploymentToManage.TaskId))
            {
                taskToManage = octRepositoryToManage.Tasks.Get(deploymentToManage.TaskId);
            }
        }

        /// <summary>
        /// A simple and efficient way to manage DeploymentTasks
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">The Task to manage.</param>
        public OctopusDeploymentTaskManager(OctopusRepository octRepository, TaskResource task)
        {
            octRepositoryToManage = octRepository;
            taskToManage = task;
            deploymentToManage = DeploymentHelper.GetDeploymentFromTask(octRepository, task);
        }

        /// <summary>
        /// Start the Deploy.
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
        /// A call to Cancel that can be managed from other areas.
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
                taskToManage = TaskHelper.GetTaskFromId(octRepositoryToManage, taskToManage.Id);
                if (Status == TaskManagerStatus.Executing || Status == TaskManagerStatus.Interrupted || Status == TaskManagerStatus.Queued || Status == TaskManagerStatus.Intervention)
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
        /// <param name="response">Response passed to the interruption.</param>
        /// <param name="note">Note passed to the interruption.</param>
        public void RespondToInterruption(InterruptionResponse response, string note)
        {
            UpdateInterruption();
            InterruptionHelper.InterruptionReponse(octRepositoryToManage, currentInterruptionToProcess, response, note);
            UpdatePreviousInterruption();
        }


        /// <summary>
        /// Refreshes the current pending interruption, if one exists.
        /// </summary>
        public void UpdateInterruption()
        {
            var updatedInterruption = InterruptionHelper.GetPendingInterruption(octRepositoryToManage, taskToManage);
            if (updatedInterruption != null && !updatedInterruption.Equals(currentInterruptionToProcess))
            {
                currentInterruptionToProcess = updatedInterruption;
            }
        }

        /// <summary>
        /// Refreshes the current pending intervention, if one exists.
        /// </summary>
        public void UpdateIntervention()
        {
            var updatedIntervention = InterruptionHelper.GetLastInterruption(octRepositoryToManage, taskToManage);
            if (updatedIntervention != null && !updatedIntervention.Equals(currentInterventionToProcess))
            {
                currentInterruptionToProcess = updatedIntervention;
            }
        }

        /// <summary>
        /// Refreshes the previous interruption, used for reporting on the guided interruption.
        /// </summary>
        private void UpdatePreviousInterruption()
        {
            previousInterruptionToProcess = octRepositoryToManage.Interruptions.Get(currentInterruptionToProcess.Id);
            while ((previousInterruptionToProcess.IsPending || previousInterruptionToProcess.ResponsibleUserId == null || previousInterruptionToProcess.Form == null || previousInterruptionToProcess.Form.Values[ResourceStrings.InterruptionGuidanceKey] == null) && Status != TaskManagerStatus.Canceled)
            {
                previousInterruptionToProcess = octRepositoryToManage.Interruptions.Get(currentInterruptionToProcess.Id);
            }
        }

        /// <summary>
        /// Refreshes the previous interruption, used for reporting on the guided interruption.
        /// </summary>
        private void UpdatePreviousIntervention()
        {
            previousInterruptionToProcess = octRepositoryToManage.Interruptions.Get(currentInterruptionToProcess.Id);
            while ((previousInterruptionToProcess.IsPending || previousInterruptionToProcess.ResponsibleUserId == null || previousInterruptionToProcess.Form == null || previousInterruptionToProcess.Form.Values[ResourceStrings.InterventionResultKey] == null) && Status != TaskManagerStatus.Canceled)
            {
                previousInterruptionToProcess = octRepositoryToManage.Interruptions.Get(currentInterruptionToProcess.Id);
            }
        }

        /// <summary>
        /// Refreshes the current list of activity.
        /// </summary>
        private void UpdateActivity()
        {
            taskToManage = TaskHelper.GetTaskFromId(octRepositoryToManage, taskToManage.Id);
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
        /// <param name="activityElementToProcess">The activity element to check for failures for.</param>
        /// <param name="tabIndex">Tab index used for indenting for levels of failure.</param>
        /// <returns>Returns the combine string of failures.</returns>
        private string GetFailures(ActivityElement activityElementToProcess, int tabIndex)
        {
            var output = string.Empty;
            var tabCount = new string(' ', tabIndex * 5);
            if (activityElementToProcess.Status == ActivityStatus.Failed || activityElementToProcess.Status == ActivityStatus.SuccessWithWarning || activityElementToProcess.Status == ActivityStatus.Running)
            {
                output += activityElementToProcess.Name + ResourceStrings.Return;
                foreach (var activityLogElement in activityElementToProcess.LogElements)
                {
                    output += (string.Format(ResourceStrings.LogPrinting, tabCount, activityLogElement.MessageText.Replace(ResourceStrings.Return, string.Format(ResourceStrings.LogPrinting, ResourceStrings.Return, tabCount)))) + ResourceStrings.Return;
                }
                foreach (var activityElement in activityElementToProcess.Children)
                {
                    output += GetFailures(activityElement, tabIndex + 1);
                }
            }
            return output.TrimEnd();
        }

        /// <summary>
        /// Returns the important information, step and sub step info, about the Errors and Warnings that occurred.
        /// </summary>
        /// <returns>Returns all errors and warnings related to the deployment task.</returns>
        public string GetErrorsAndWarnings()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.Failed || a.Status == ActivityStatus.SuccessWithWarning))
            {
                output += GetFailures(activityStep, 0) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        /// <summary>
        /// Returns the important information, step and sub step info, about the Warnings that occurred.
        /// </summary>
        /// <returns>Returns all warnings related to the deployment task.</returns>
        public string GetWarnings()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.SuccessWithWarning))
            {
                output += GetFailures(activityStep, 0) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        /// <summary>
        /// Returns the important information, step and sub step info, about the Errors that occurred.
        /// </summary>
        /// <returns>Returns all errors related to the deployment task.</returns>
        public string GetErrors()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => a.Status == ActivityStatus.Failed))
            {
                output += GetFailures(activityStep, 0) + ResourceStrings.Return;
            }
            return output.Trim();
        }

        /// <summary>
        /// Gets the User that canceled the Deployment
        /// </summary>
        /// <returns>Returns the UserResource of the user who initiated the task cancellation.</returns>
        public UserResource GetCancellingUser()
        {
            if(taskToManage.State == TaskState.Canceled || taskToManage.State == TaskState.Cancelling)
            {
                var cancellingEventList = EventHelper.GetResourceEvents(octRepositoryToManage, taskToManage.Id, ResourceStrings.CancelledTaskEventCategory).ToList();
                if(cancellingEventList.Count() == 0)
                {
                    return null;
                }
                var cancellingEvent = cancellingEventList.OrderByDescending(x => x.LastModifiedOn).FirstOrDefault();
                return UserHelper.GetUserFromUserId(octRepositoryToManage, cancellingEvent.UserId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the output for logging, only gathers non-previously reported info.
        /// </summary>
        /// <returns>Re tuns the log of Main steps.</returns>
        public string GetLog()
        {
            UpdateActivity();
            var output = string.Empty;
            foreach (var activityStep in activitySteps.Where(a => !printedLog.Any(p => p.Key.Equals(a.Id, StringComparison.OrdinalIgnoreCase))))
            {
                if (activityStep.Status != ActivityStatus.Pending)
                {
                    printedLog.Add(activityStep.Id, activityStep.Name);
                    output += activityStep.Name + ResourceStrings.Return;
                }
            }
            return output.Trim();
        }

        /// <summary>
        /// Gets the current Step count.
        /// </summary>
        /// <returns>Re tuns the total number of steps.</returns>
        public int GetStepCount()
        {
            return activitySteps.Count();
        }

        /// <summary>
        /// Gets the currently completed Step count.
        /// </summary>
        /// <returns>Returns the total number of completed steps.</returns>
        public int GetStepCompletedCount()
        {
            return printedLog.Count();
        }

        /// <summary>
        /// Gets the current count of deployments ahead of the current one.
        /// If this returns 0 and the status is queued, then that means that it has hit the node limit of deployments
        /// TO DO: add consideration for reaching server node max deployment amount. need to investigate what is considered an active task.
        /// </summary>
        /// <returns>Returns the total number of tasks ahead of this one.</returns>
        public int GetQueuedDeploymentCount()
        {
            var count = DeploymentHelper.GetQueuedDeployments(octRepositoryToManage, deploymentToManage).Count();
            return count;
        }

        /// <summary>
        /// Get the Step the failure occurred on for Interruptions.
        /// </summary>
        /// <returns>Returns the failure of the interrupted step.</returns>
        public string GetInterruptedStepInfo()
        {
            UpdateActivity();
            var output = string.Empty;
            output += InterruptionHelper.GetInterruptedStepName(octRepositoryToManage, currentInterruptionToProcess);
            output += GetFailures(activitySteps.Where(a => a.Status == ActivityStatus.Running).FirstOrDefault().Children.LastOrDefault(), 0) + ResourceStrings.Return;
            return output.Trim();
        }

        /// <summary>
        /// Get the Step of the Manual Intervention.
        /// </summary>
        /// <returns></returns>
        public string GetManualInterventionStepInfo()
        {
            var output = string.Empty;
            output = InterruptionHelper.GetInterruptedStepName(octRepositoryToManage, currentInterruptionToProcess);
            return output;
        }

        public string GetManualInterventionDirections()
        {
            var output = string.Empty;
            output = InterruptionHelper.GetInterventionDirections(octRepositoryToManage, currentInterruptionToProcess);
            return output;
        }

        /// <summary>
        /// Returns the Last Step Executed.
        /// </summary>
        /// <returns>Returns the Last Step Executed</returns>
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
        /// Gets the Note of the Guided Interruption.
        /// </summary>
        /// <returns>Returns the Note of the managed interruption.</returns>
        public string GetManagedInterruptionNote()
        {
            UpdatePreviousInterruption();
            if (previousInterruptionToProcess != null)
            {
                return previousInterruptionToProcess.Form.Values[ResourceStrings.InterruptionNoteKey];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Note of the Manual Intervention.
        /// </summary>
        /// <returns>Returns the Note of the manual intervention.</returns>
        public string GetManualInterventionNote()
        {
            UpdatePreviousIntervention();
            if (previousInterruptionToProcess != null)
            {
                return previousInterruptionToProcess.Form.Values[ResourceStrings.InterventionNoteKey];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Guidance Taken of the Guided Interruption.
        /// </summary>
        /// <returns>Returns the value of the choice (retry, fail, or cancel) of the of the managed interruption.</returns>
        public string GetManagedInterruptionGuidence()
        {
            UpdatePreviousInterruption();
            if (previousInterruptionToProcess != null)
            {
                return previousInterruptionToProcess.Form.Values[ResourceStrings.InterruptionGuidanceKey];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Guidance Taken of the Manual Intervention.
        /// </summary>
        /// <returns>Returns the value of the choice (proceed or abort) of the of the manual intervention.</returns>
        public string GetManualInterventionGuidence()
        {
            UpdatePreviousIntervention();
            if (previousInterruptionToProcess != null)
            {
                return previousInterruptionToProcess.Form.Values[ResourceStrings.InterventionResultKey];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Guiding User of the Guided Interruption.
        /// </summary>
        /// <returns>Returns the Responsible UserResource of the interruption</returns>
        public UserResource GetManagedInterruptionResponsibleUser()
        {
            UpdatePreviousInterruption();
            if (previousInterruptionToProcess != null)
            {
                return UserHelper.GetUserFromUserId(octRepositoryToManage, previousInterruptionToProcess.ResponsibleUserId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Guiding User of the Manual Intervention.
        /// </summary>
        /// <returns>Returns the Responsible UserResource of the intervention</returns>
        public UserResource GetManualInterventionResponsibleUser()
        {
            UpdatePreviousIntervention();
            if (previousInterruptionToProcess != null)
            {
                return UserHelper.GetUserFromUserId(octRepositoryToManage, previousInterruptionToProcess.ResponsibleUserId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check if there there is a new interruption.
        /// </summary>
        public bool HasNewInterruption
        {
            get
            {
                return previousInterruptionToProcess != null && currentInterruptionToProcess.Id != previousInterruptionToProcess.Id;
            }
        }

        /// <summary>
        /// Returns the server link to the deployment.
        /// </summary>
        /// <returns>Returns the server link to the deployment.</returns>
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
        /// Gets the current Status of the deployment.
        /// </summary>
        public TaskManagerStatus Status
        {
            get
            {
                var currentState = TaskManagerStatus.NotStarted;
                if (taskToManage != null)
                {
                    taskToManage = TaskHelper.GetTaskFromId(octRepositoryToManage, taskToManage.Id);
                    if (taskToManage.HasPendingInterruptions)
                    {
                        //We have to wait since we cant tell if an interruption is a guided failure step, or a manual intervention
                        Thread.Sleep(1000);
                        taskToManage = TaskHelper.GetTaskFromId(octRepositoryToManage, taskToManage.Id);
                        UpdateInterruption();
                        if (taskToManage.HasPendingInterruptions && taskToManage.State == TaskState.Queued)
                        {
                            currentState = TaskManagerStatus.Intervention;
                        }
                        else if (taskToManage.HasPendingInterruptions && taskToManage.State == TaskState.Executing)
                        {
                            currentState = TaskManagerStatus.Interrupted;
                        }

                    }
                    else
                    {
                        if (taskToManage.IsCompleted && taskToManage.State != TaskState.Canceled)
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
                                case TaskState.TimedOut:
                                    currentState = TaskManagerStatus.TimedOut;
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
                }
                return currentState;
            }
        }

        /// <summary>
        /// Gets the status of the Cancellation.
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
        /// Gets the Error Status of the Task.
        /// </summary>
        public ErrorStatus ErrorStatus
        {
            get
            {
                UpdateActivity();
                var errorState = ErrorStatus.None;
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
