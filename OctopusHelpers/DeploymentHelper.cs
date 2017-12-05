using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Forms;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Deployment Objects.
    /// </summary>
    public static class DeploymentHelper
    {
        /// <summary>
        /// Gathering the link needed for displaying the deployment in the front end.
        /// </summary>
        /// <param name="deployment">The Deployment resource</param>
        /// <returns>The URL of the deployment passed.</returns>
        public static string GetDeploymentLinkForWeb(DeploymentResource deployment)
        {
            return string.Format(ResourceStrings.OctopusDeploymentLink, deployment.Id);
        }

        /// <summary>
        /// Gathers the Deployment Variables needed for a deployment
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">The release to gather the variables from</param>
        /// <param name="environment">The environment used to gather the variables from</param>
        /// <returns>Dictionary of DeploymentVariables.</returns>
        public static IDictionary<string, string> GetDeploymentVariables(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment)
        {
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            var deploymentPreview = octRepository.Releases.GetPreview(deploymentPromotionTarget);
            return deploymentPreview.Form.Values;
        }

        /// <summary>
        /// Builds a DeploymentResource for the release and environment passed, creates what steps to skip based on a string list.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to deploy.</param>
        /// <param name="environment">Environment to deploy to.</param>
        /// <param name="formValues">Form value variable dictionary.</param>
        /// <param name="guidedFailure">Enable Guided Failure.</param>
        /// <param name="skippedSteps">Steps to skip.</param>
        /// <param name="dateToDeploy">Deployment Date.</param>
        /// <returns>DeploymentResource.</returns>
        public static DeploymentResource BuildDeployment(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment, Dictionary<string, string> formValues, bool guidedFailure, IEnumerable<string> skippedSteps, DateTimeOffset? dateToDeploy)
        {
            var machineIDs = new ReferenceCollection();
            var skippedStepIDs = new ReferenceCollection();
            var projectSteps = StepHelper.GetReleaseEnvironmentDeploymentSteps(octRepository, release, environment);
            var skippedStepList = projectSteps.Where(p => skippedSteps.Any(s => s.Equals(p.ActionName, StringComparison.OrdinalIgnoreCase))).Select(d => d.ActionId);
            skippedStepIDs.ReplaceAll(skippedStepList);
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            var deploymentPreview = octRepository.Releases.GetPreview(deploymentPromotionTarget);

            foreach(var element in deploymentPreview.Form.Elements)
            {
                var variableInput = element.Control as VariableValue;
                if (variableInput != null)
                {
                    var variableValue = formValues[variableInput.Label] ?? formValues[variableInput.Name];
                    if(string.IsNullOrWhiteSpace(variableValue) && element.IsValueRequired)
                    {
                        throw new ArgumentException(string.Format(ErrorStrings.MissingRequiredVar, variableInput.Label ?? variableInput.Name, ResourceStrings.FormValuesArgException));
                    }
                }
            }
            var deploymentResource = new DeploymentResource
            {
                EnvironmentId = environment.Id,
                SkipActions = skippedStepIDs,
                ReleaseId = release.Id,
                ForcePackageDownload = false,
                UseGuidedFailure = guidedFailure,
                SpecificMachineIds = machineIDs,
                ForcePackageRedeployment = true,
                FormValues = formValues,
                QueueTime = dateToDeploy
            };
            return deploymentResource;
        }

        /// <summary>
        /// Builds a DeploymentResource for the release and environment passed, creates what steps to skip based on a string list.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to deploy.</param>
        /// <param name="environment">Environment to deploy to.</param>
        /// <param name="comment">Comment for the deployment.</param>
        /// <param name="formValues">Form value variable dictionary.</param>
        /// <param name="guidedFailure">Enable Guided Failure.</param>
        /// <param name="skippedSteps">Steps to skip.</param>
        /// <param name="dateToDeploy">Deployment Date.</param>
        /// <returns>DeploymentResource.</returns>
        public static DeploymentResource BuildDeployment(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment, string comment, Dictionary<string, string> formValues, bool guidedFailure, IEnumerable<string> skippedSteps, DateTimeOffset? dateToDeploy)
        {
            var machineIDs = new ReferenceCollection();
            var skippedStepIDs = new ReferenceCollection();
            var projectSteps = StepHelper.GetReleaseEnvironmentDeploymentSteps(octRepository, release, environment);
            var skippedStepList = projectSteps.Where(p => skippedSteps.Any(s => s.Equals(p.ActionName, StringComparison.OrdinalIgnoreCase))).Select(d => d.ActionId);
            skippedStepIDs.ReplaceAll(skippedStepList);
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            var deploymentPreview = octRepository.Releases.GetPreview(deploymentPromotionTarget);

            foreach (var element in deploymentPreview.Form.Elements)
            {
                var variableInput = element.Control as VariableValue;
                if (variableInput != null)
                {
                    var variableValue = formValues[variableInput.Label] ?? formValues[variableInput.Name];
                    if (string.IsNullOrWhiteSpace(variableValue) && element.IsValueRequired)
                    {
                        throw new ArgumentException(string.Format(ErrorStrings.MissingRequiredVar, variableInput.Label ?? variableInput.Name, ResourceStrings.FormValuesArgException));
                    }
                }
            }
            var deploymentResource = new DeploymentResource
            {
                EnvironmentId = environment.Id,
                SkipActions = skippedStepIDs,
                ReleaseId = release.Id,
                ForcePackageDownload = false,
                UseGuidedFailure = guidedFailure,
                SpecificMachineIds = machineIDs,
                ForcePackageRedeployment = true,
                FormValues = formValues,
                QueueTime = dateToDeploy,
                Comments = comment
            };
            return deploymentResource;
        }

        /// <summary>
        /// Builds a DeploymentResource for the release and environment passed, allows you to directly pass the list of skipped steps
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to deploy.</param>
        /// <param name="environment">Environment to deploy to.</param>
        /// <param name="formValues">Form value variable dictionary.</param>
        /// <param name="guidedFailure">Enable Guided Failure.</param>
        /// <param name="skippedSteps">Steps to skip.</param>
        /// <param name="dateToDeploy">Deployment Date.</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource BuildDeployment(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment, Dictionary<string, string> formValues, bool guidedFailure, ReferenceCollection skippedSteps, DateTimeOffset? dateToDeploy)
        {
            var machineIDs = new ReferenceCollection();
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            var deploymentPreview = octRepository.Releases.GetPreview(deploymentPromotionTarget);

            foreach (var element in deploymentPreview.Form.Elements)
            {
                var variableInput = element.Control as VariableValue;
                if (variableInput != null)
                {
                    var variableValue = formValues[variableInput.Label] ?? formValues[variableInput.Name];
                    if (string.IsNullOrWhiteSpace(variableValue) && element.IsValueRequired)
                    {
                        throw new ArgumentException(string.Format(ErrorStrings.MissingRequiredVar, variableInput.Label ?? variableInput.Name, ResourceStrings.FormValuesArgException));
                    }
                }
            }
            var deploymentResource = new DeploymentResource
            {
                EnvironmentId = environment.Id,
                SkipActions = skippedSteps,
                ReleaseId = release.Id,
                ForcePackageDownload = false,
                UseGuidedFailure = guidedFailure,
                SpecificMachineIds = machineIDs,
                ForcePackageRedeployment = true,
                FormValues = formValues,
                QueueTime = dateToDeploy
            };
            return deploymentResource;
        }



        /// <summary>
        /// Builds a DeploymentResource for the release and environment passed, allows you to directly pass the list of skipped steps
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to deploy.</param>
        /// <param name="environment">Environment to deploy to.</param>
        /// <param name="comment">Comment for the deployment.</param>
        /// <param name="formValues">Form value variable dictionary.</param>
        /// <param name="guidedFailure">Enable Guided Failure.</param>
        /// <param name="skippedSteps">Steps to skip.</param>
        /// <param name="dateToDeploy">Deployment Date.</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource BuildDeployment(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment, string comment, Dictionary<string, string> formValues, bool guidedFailure, ReferenceCollection skippedSteps, DateTimeOffset? dateToDeploy)
        {
            var machineIDs = new ReferenceCollection();
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            var deploymentPreview = octRepository.Releases.GetPreview(deploymentPromotionTarget);

            foreach (var element in deploymentPreview.Form.Elements)
            {
                var variableInput = element.Control as VariableValue;
                if (variableInput != null)
                {
                    var variableValue = formValues[variableInput.Label] ?? formValues[variableInput.Name];
                    if (string.IsNullOrWhiteSpace(variableValue) && element.IsValueRequired)
                    {
                        throw new ArgumentException(string.Format(ErrorStrings.MissingRequiredVar, variableInput.Label ?? variableInput.Name, ResourceStrings.FormValuesArgException));
                    }
                }
            }
            var deploymentResource = new DeploymentResource
            {
                EnvironmentId = environment.Id,
                SkipActions = skippedSteps,
                ReleaseId = release.Id,
                ForcePackageDownload = false,
                UseGuidedFailure = guidedFailure,
                SpecificMachineIds = machineIDs,
                ForcePackageRedeployment = true,
                FormValues = formValues,
                QueueTime = dateToDeploy,
                Comments = comment
            };
            return deploymentResource;
        }

        /// <summary>
        /// Returns a list of Queued deployments ahead of the passed Deployment, this includes the currently executing one.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="deployment">Deployment to check what is queued ahead of it.</param>
        /// <returns>Enumerable of DeploymentResources</returns>
        public static IEnumerable<DeploymentResource> GetQueuedDeployments(OctopusRepository octRepository, DeploymentResource deployment)
        {
            var deploymentsList = new List<DeploymentResource>();
            var task = octRepository.Tasks.Get(deployment.TaskId);
            var queuedBehindTask = octRepository.Tasks.GetQueuedBehindTasks(task).Where(x => x.Arguments.ContainsKey(ResourceStrings.DeploymentIdKey)).Select(x => x.Arguments[ResourceStrings.DeploymentIdKey].ToString());
            foreach(var queuedDeployment in queuedBehindTask)
            {
                deploymentsList.Add(octRepository.Deployments.Get(queuedDeployment));
            }
            return deploymentsList;
        }

        /// <summary>
        /// Gathers the last deployment of the passed releases.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to gather the last deployment of.</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource GetLastDeploymentOfRelease(OctopusRepository octRepository, ReleaseResource release)
        {
            var releaseDeployments = octRepository.Client.GetReleaseDeployments(release);
            if(releaseDeployments != null && releaseDeployments.Count() > 0)
            {
                return releaseDeployments.OrderBy(x => x.Created).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the last deployment of the passed releases for the specified environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to call against.</param>
        /// <param name="environment">Environment to call against.</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource GetLastDeploymentOfEnvironmentRelease(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment)
        {
            var releaseDeployments = octRepository.Client.GetReleaseDeployments(release);
            if (releaseDeployments != null && releaseDeployments.Count() > 0)
            {
                return releaseDeployments.Where(x => x.EnvironmentId == environment.Id).OrderBy(x => x.Created).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the last non active deployment of the passed releases for the specified environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to call against.</param>
        /// <param name="environment">Environment to call against.</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource GetNonActiveLastDeploymentOfEnvironmentRelease(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment)
        {
            var nonActiveStatusList = new List<TaskState>()
            {
                TaskState.Queued,
                TaskState.Executing
            };
            var releaseDeployments = octRepository.Client.GetReleaseDeployments(release);
            if (releaseDeployments != null && releaseDeployments.Count() > 0)
            {
                var nonActiveReleaseDeployments = releaseDeployments.Where(x => x.EnvironmentId == environment.Id && !(nonActiveStatusList.Contains(TaskHelper.GetTaskFromId(octRepository, x.TaskId).State)));
                if (nonActiveReleaseDeployments != null && nonActiveReleaseDeployments.Count() > 0)
                {
                    var lastDeployment = nonActiveReleaseDeployments.OrderBy(x => x.Created).FirstOrDefault();
                    return lastDeployment;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers deployment of the passed releases for the specified environment of the specified state.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to call against.</param>
        /// <param name="environment">Environment to call against.</param>
        /// <param name="state">State of the Task</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource GetLastDeploymentOfEnvironmentReleaseByState(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment, TaskState state)
        {
            var releaseDeployments = octRepository.Client.GetReleaseDeployments(release);
            if (releaseDeployments != null && releaseDeployments.Count() > 0)
            {
                return releaseDeployments.Where(x => x.EnvironmentId == environment.Id && TaskHelper.GetTaskFromId(octRepository, x.TaskId).State == state).OrderBy(x => x.Created).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the variables associated to a deployment
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to call against.</param>
        /// <param name="environment">Environment to call against.</param>
        /// <returns>DeploymentResource</returns>
        public static IEnumerable<string> GetDeploymentFormVariables(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment)
        {
            var varList = new List<string>();
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            var deploymentPreview = octRepository.Releases.GetPreview(deploymentPromotionTarget);
            foreach (var element in deploymentPreview.Form.Elements)
            {
                var variableInput = element.Control as VariableValue;
                if (variableInput != null)
                {
                    varList.Add(variableInput.Label);
                }
            }
            return varList;
        }

        /// <summary>
        /// Gathers the deployments 
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to call against.</param>
        /// <param name="environment">Environment to call against.</param>
        /// <returns>Enumerable of DeploymentResources</returns>
        public static IEnumerable<DeploymentResource> GetProjectEnvironmentDeployments(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var lastDeployedReleases = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray).ToList();
            return lastDeployedReleases;
        }

        /// <summary>
        /// Gathers the Deployment From the Task Passed
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather deployment from.</param>
        /// <returns>DeploymentResource</returns>
        public static DeploymentResource GetDeploymentFromTask(OctopusRepository octRepository, TaskResource task)
        {
            var deployment = new DeploymentResource();
            deployment = octRepository.Deployments.Get(task.Arguments[ResourceStrings.DeploymentIdKey].ToString());
            return deployment;
        }
    }
}
