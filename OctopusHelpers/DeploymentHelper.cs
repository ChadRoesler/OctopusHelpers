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
        /// <returns>The url of the deployment passed.</returns>
        public static string GetDeploymentLinkForWeb(DeploymentResource deployment)
        {
            return string.Format(ResourceStrings.OctopusDeploymentLink, deployment.Id);
        }

        /// <summary>
        /// Gathers the Deployment Variables needed for a deployment
        /// </summary>
        /// <param name="octRepository">The orelease to grab the variables from</param>
        /// <param name="release"></param>
        /// <param name="environment"></param>
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
        /// <param name="release"></param>
        /// <param name="environment"></param>
        /// <param name="formValues"></param>
        /// <param name="guidedFailure"></param>
        /// <param name="skippedSteps"></param>
        /// <param name="dateToDeploy"></param>
        /// <returns>Creates the DeploymentResource.</returns>
        public static DeploymentResource BuildDeployment(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment, Dictionary<string, string> formValues, bool guidedFailure, IEnumerable<string> skippedSteps, DateTimeOffset? dateToDeploy)
        {
            var machineIDs = new ReferenceCollection();
            var skippedStepIDs = new ReferenceCollection();
            var projectSteps = StepHelper.GetProjectEnvironmentDeploymentSteps(octRepository, release, environment);
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
        /// Builds a DeploymentResource for the release and environment passed, allows you to directly pass the list of skipped steps
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release"></param>
        /// <param name="environment"></param>
        /// <param name="formValues"></param>
        /// <param name="guidedFailure"></param>
        /// <param name="skippedSteps"></param>
        /// <param name="dateToDeploy"></param>
        /// <returns></returns>
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
        /// Returns a list of Queued deployements ahead of the passed Deployment, this in cludes the currently executing one.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="deployment"></param>
        /// <returns></returns>
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
        /// Returns the variables associated to a deployment
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
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
    }
}
