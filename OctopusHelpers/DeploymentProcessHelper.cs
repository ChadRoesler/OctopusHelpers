using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class DeploymentProcessHelper
    {
        /// <summary>
        /// Gathers the DeploymentProcessResource from a Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static DeploymentProcessResource GetDeploymentProcessFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var deploymentProcess = octRepository.DeploymentProcesses.Get(project.DeploymentProcessId);
            return deploymentProcess;
        }

        /// <summary>
        /// returns step names that gather packages from nuget feeds.
        /// </summary>
        /// <param name="deploymentProcess"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPackageSteps(DeploymentProcessResource deploymentProcess)
        {
            return deploymentProcess.Steps.SelectMany(x => x.Actions).Where(y => y.ActionType.Equals(ResourceStrings.PackageActionType, StringComparison.OrdinalIgnoreCase)).Select(x => x.Name);
        }

        /// <summary>
        /// Updates the new Deployment Process with the info from another. (Look you cant update the Steps property, its read only for some reason)
        /// This ha changed but this is something ill need to look into more
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="newDeploymentProcess"></param>
        /// <param name="oldDeploymentProcess"></param>
        public static void UpdateDeploymentProcessFromDeploymentProcess(OctopusRepository octRepository, DeploymentProcessResource newDeploymentProcess, DeploymentProcessResource oldDeploymentProcess)
        {
            newDeploymentProcess.Id = oldDeploymentProcess.Id;
            newDeploymentProcess.ProjectId = oldDeploymentProcess.ProjectId;
            newDeploymentProcess.Version = oldDeploymentProcess.Version;
            newDeploymentProcess.Links = oldDeploymentProcess.Links;
            octRepository.DeploymentProcesses.Modify(newDeploymentProcess);
        }

        /// <summary>
        /// Updates the passed Project's Deployment Process
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="deploymentProcess"></param>
        public static void UpdateProjectDeploymentProcess(OctopusRepository octRepository, ProjectResource project, DeploymentProcessResource deploymentProcess)
        {
            var oldDeploymentProcess = GetDeploymentProcessFromProject(octRepository, project);
            UpdateDeploymentProcessFromDeploymentProcess(octRepository, deploymentProcess, oldDeploymentProcess);
        }
    }
}
