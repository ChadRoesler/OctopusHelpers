using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;
using Semver;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Release Objects.
    /// </summary>
    public static class ReleaseHelper
    {
        /// <summary>
        /// Get all releases of a Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather releases from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetProjectReleases(OctopusRepository octRepository, ProjectResource project)
        {
            return octRepository.Projects.GetAllReleases(project);
        }

        /// <summary>
        /// Get releases of the passed channel.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="channel">Channel to gather from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetChannelReleases(OctopusRepository octRepository, ChannelResource channel)
        {
            return octRepository.Client.GetChannelReleases(channel);
        }

        /// <summary>
        /// Gather a Project's Release from the passed Version.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather release from.</param>
        /// <param name="releaseVersion">Version to gather of release.</param>
        /// <returns>ReleaseResource</returns>
        public static ReleaseResource GetProjectReleaseByVersion(OctopusRepository octRepository, ProjectResource project, SemVersion releaseVersion)
        {
            var projectReleases = GetProjectReleases(octRepository, project);
            if (projectReleases != null && projectReleases.Count() > 0)
            {
                var releaseList = projectReleases.Where(r => r.GetVersionObject().Equals(releaseVersion)).FirstOrDefault();
                return releaseList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a release for the passed Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to create release for.</param>
        /// <param name="releaseVersion">Release version to create.</param>
        /// <param name="stepAndVersionDictionary">StepName and Version for package steps.</param>
        /// <param name="releaseNotes">Release Notes.</param>
        /// <returns>Newly Created ReleaseResource</returns>
        public static ReleaseResource CreateProjectRelease(OctopusRepository octRepository, ProjectResource project, string releaseVersion, Dictionary<string, SemVersion> stepAndVersionDictionary, string releaseNotes)
        {

            var release = new ReleaseResource();
            var projectDeploymentProcess = DeploymentProcessHelper.GetDeploymentProcessFromProject(octRepository, project);
            var packageSteps = DeploymentProcessHelper.GetPackageSteps(projectDeploymentProcess);
            var package = new List<SelectedPackage>();
            foreach(var step in packageSteps)
            {
                if(stepAndVersionDictionary.ContainsKey(step))
                {
                    package.Add(new SelectedPackage(step, stepAndVersionDictionary[step].ToString()));
                }
                else
                {
                    throw new ArgumentException(string.Format(ErrorStrings.MissingPackageStep, step), "stepAndVersionDictionary");
                }
            }
            release.SelectedPackages = package;
            release.ReleaseNotes = releaseNotes;
            release.Version = releaseVersion;
            release.ProjectId = project.Id;
            return octRepository.Releases.Create(release);
        }

        /// <summary>
        /// Creates a release for the passed Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to create release for.</param>
        /// <param name="releaseVersion">Release version to create.</param>
        /// <param name="releaseNotes">Release Notes.</param>
        /// <returns>Newly Created ReleaseResource</returns>
        public static ReleaseResource CreateProjectRelease(OctopusRepository octRepository, ProjectResource project, string releaseVersion, string releaseNotes)
        {
            var release = new ReleaseResource();
            var projectDeploymentProcess = DeploymentProcessHelper.GetDeploymentProcessFromProject(octRepository, project);
            release.ReleaseNotes = releaseNotes;
            release.Version = releaseVersion;
            release.ProjectId = project.Id;
            return octRepository.Releases.Create(release);
        }

        /// <summary>
        /// Creates a release for the passed Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to create release for.</param>
        /// <param name="channel">Channel to create release for.</param>
        /// <param name="releaseVersion">Release version to create.</param>
        /// <param name="stepAndVersionDictionary">StepName and Version for package steps.</param>
        /// <param name="releaseNotes">Release Notes.</param>
        /// <returns>Newly Created ReleaseResource</returns>
        public static ReleaseResource CreateProjectRelease(OctopusRepository octRepository, ProjectResource project, ChannelResource channel, string releaseVersion, Dictionary<string, SemVersion> stepAndVersionDictionary, string releaseNotes)
        {

            var release = new ReleaseResource();
            var projectDeploymentProcess = DeploymentProcessHelper.GetDeploymentProcessFromProject(octRepository, project);
            var packageSteps = DeploymentProcessHelper.GetPackageSteps(projectDeploymentProcess);
            var package = new List<SelectedPackage>();
            foreach (var step in packageSteps)
            {
                if (stepAndVersionDictionary.ContainsKey(step))
                {
                    package.Add(new SelectedPackage(step, stepAndVersionDictionary[step].ToString()));
                }
                else
                {
                    throw new ArgumentException(string.Format(ErrorStrings.MissingPackageStep, step), "stepAndVersionDictionary");
                }
            }
            release.ChannelId = channel.Id;
            release.SelectedPackages = package;
            release.ReleaseNotes = releaseNotes;
            release.Version = releaseVersion;
            release.ProjectId = project.Id;
            return octRepository.Releases.Create(release);
        }

        /// <summary>
        /// Creates a release for the passed Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to create release for.</param>
        /// <param name="channel">Channel to create release for.</param>
        /// <param name="releaseVersion">Release version to create.</param>
        /// <param name="releaseNotes">Release Notes.</param>
        /// <returns>Newly Created ReleaseResource</returns>
        /// <returns></returns>
        public static ReleaseResource CreateProjectRelease(OctopusRepository octRepository, ProjectResource project, ChannelResource channel, string releaseVersion, string releaseNotes)
        {

            var release = new ReleaseResource();
            var projectDeploymentProcess = DeploymentProcessHelper.GetDeploymentProcessFromProject(octRepository, project);
            release.ChannelId = channel.Id;
            release.ReleaseNotes = releaseNotes;
            release.Version = releaseVersion;
            release.ProjectId = project.Id;
            return octRepository.Releases.Create(release);
        }

        /// <summary>
        /// Creates a dummy release and deletes it on the passed Project for deployment process updating.
        /// This is an old way of doing it.  Its been replaced by updating the SQL database: https://g.rrrather.com/img/q/31410a.jpg
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to create dummy release.</param>
        public static void CreateDeleteDummyReleaseForProject(OctopusRepository octRepository, ProjectResource project)
        {
            var release = new ReleaseResource();
            var projectDeploymentProcess = DeploymentProcessHelper.GetDeploymentProcessFromProject(octRepository, project);
            var packageSteps = DeploymentProcessHelper.GetPackageSteps(projectDeploymentProcess);
            var package = new List<SelectedPackage>();
            foreach(var step in packageSteps)
            {
                package.Add(new SelectedPackage(step, ResourceStrings.DummyPackageVersion));
            }
            release.SelectedPackages = package;
            release.ReleaseNotes = ResourceStrings.DummyPackageVersion;
            release.Version = ResourceStrings.DummyReleaseVersion;
            release.ProjectId = project.Id;
            var dummyRelease = octRepository.Releases.Create(release);
            DeleteRelease(octRepository, dummyRelease);
        }

        /// <summary>
        /// Deletes the passed Release.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to Delete.</param>
        public static void DeleteRelease(OctopusRepository octRepository, ReleaseResource release)
        {
            octRepository.Releases.Delete(release);
        }

        /// <summary>
        /// Updates the Variables for the passed Release.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to update.</param>
        public static void UpdateReleaseVariables(OctopusRepository octRepository, ReleaseResource release)
        {
            octRepository.Releases.SnapshotVariables(release);
        }

        /// <summary>
        /// Gathers a list of deployed Releases for a passed Project and Environment. (This has to be done by ReleaseId due to not having another way to connect deployed releases to release resources).
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to Gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetDeployedReleasesFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var releaseList = new List<ReleaseResource>();
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var deployments = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray).ToList();
            if (deployments != null && deployments.Count > 0)
            {
                var releases = GetProjectReleases(octRepository, project);
                if (releases != null && releases.Count() > 0)
                {
                    releaseList.AddRange(releases.Where(x => deployments.Any(d => d.ReleaseId.Equals(x.Id))).ToList());
                }
            }
            return releaseList;
        }

        /// <summary>
        /// Gathers a list of last deployed releases per environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to Gather from.</param>
        /// <param name="phaseName">Phase name to gather from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetLastDeployedReleasesFromProjectPhase(OctopusRepository octRepository, ProjectResource project, string phaseName)
        {
            var releaseList = new List<ReleaseResource>();
            var environmentList = EnvironmentHelper.GetProjectEnvironments(octRepository, project, phaseName);
            foreach (var environment in environmentList)
            {
                var lastDeployedPerEnvironment = GetLastDeployedReleaseFromProjectEnvironment(octRepository, project, environment);
                if (lastDeployedPerEnvironment != null && !releaseList.Contains(lastDeployedPerEnvironment))
                {
                    releaseList.Add(lastDeployedPerEnvironment);
                }
            }
            return releaseList;
        }


        /// <summary>
        /// Gathers the last deployed Release from the passed Project and Environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <returns>ReleaseResource</returns>
        public static ReleaseResource GetLastDeployedReleaseFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var lastDeployedReleases = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray).ToList();
            if (lastDeployedReleases != null && lastDeployedReleases.Count > 0)
            {
                var lastDeployedReleaseId = lastDeployedReleases.OrderByDescending(p => p.Created).FirstOrDefault().ReleaseId;
                var lastDeployedRelease = octRepository.Releases.Get(lastDeployedReleaseId);
                return lastDeployedRelease;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the last deployed release that is not currently deploying
        /// Look i know its gross, but sometimes you gotta do this.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <param name="currentTask">Current Task to Skip.</param>
        /// <returns>ReleaseResource</returns>
        public static ReleaseResource GetLastDeployedNotCurrentlyDeployingReleaseFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment, TaskResource currentTask)
        {
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var lastDeployedReleases = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray).ToList();
            if (lastDeployedReleases != null && lastDeployedReleases.Count > 0)
            {
                if (currentTask != null)
                {
                    var excludeCurrent = lastDeployedReleases.Where(x => x.TaskId != currentTask.Id);
                    var lastDeployment = excludeCurrent.OrderByDescending(p => p.Created).FirstOrDefault();
                    if (lastDeployment != null)
                    {
                        var lastDeployedRelease = octRepository.Releases.Get(lastDeployment.ReleaseId);
                        return lastDeployedRelease;
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
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the current active deployment from the passed project and environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <returns>ReleaseResource</returns>
        public static ReleaseResource GetCurrentActiveDeployedReleaseFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var lastDeployedReleases = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray);
            if (lastDeployedReleases != null && lastDeployedReleases.Count() > 0)
            {
                var executingStatusDeployedment = lastDeployedReleases.Where(x => TaskHelper.GetTaskFromId(octRepository, x.TaskId).State == TaskState.Executing).FirstOrDefault();
                var executingRelease =  octRepository.Releases.Get(executingStatusDeployedment.ReleaseId);
                return executingRelease;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers the last non active Deployed Release from the passed projet and environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <returns>ReleaseResource</returns>
        public static ReleaseResource GetLastNonActiveDeployedReleaseFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var nonActiveStatusList = new List<TaskState>()
            {
                TaskState.Queued,
                TaskState.Executing
            };
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var lastDeployedReleases = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray);
            if (lastDeployedReleases != null && lastDeployedReleases.Count() > 0)
            {
                var nonActiveStatusDeployedReleases = lastDeployedReleases.Where(x => !(nonActiveStatusList.Contains(TaskHelper.GetTaskFromId(octRepository, x.TaskId).State)));
                if (nonActiveStatusDeployedReleases != null && nonActiveStatusDeployedReleases.Count() > 0)
                {
                    var lastDeployment = lastDeployedReleases.OrderByDescending(p => p.Created).First();
                    var lastDeployedRelease = octRepository.Releases.Get(lastDeployment.ReleaseId);
                    return lastDeployedRelease;
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
        /// Gathers a list of undeployed Releases from the passed Project and Environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetUndeployedReleasesFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var releaseList = new List<ReleaseResource>();
            var deployedReleases = GetDeployedReleasesFromProjectEnvironment(octRepository, project, environment);
            var projectReleases = GetProjectReleases(octRepository, project);
            releaseList.AddRange(projectReleases.Where(r => !deployedReleases.Any(d => d.GetVersionObject().Equals(r.GetVersionObject()))).ToList());
            return releaseList;
        }

        /// <summary>
        /// Gathers a list of deployed Releases from the passed Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="phaseName">phae name to gather from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetDeployedReleasesFromProjectPhase(OctopusRepository octRepository, ProjectResource project, string phaseName)
        {
            var releaseList = new List<ReleaseResource>();
            var environmentList = EnvironmentHelper.GetProjectEnvironments(octRepository, project, phaseName);
            foreach (var environment in environmentList)
            {
                var deployedReleases = GetDeployedReleasesFromProjectEnvironment(octRepository, project, environment);
                releaseList.AddRange(deployedReleases.Where(x => !releaseList.Any(d => d.Id.Equals(x.Id))));
            }
            return releaseList;
        }

        /// <summary>
        /// Gathers a list of undeployed releases from the passed project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="phaseName">Environment to gather from.</param>
        /// <returns>Enumerable of ReleaseResources</returns>
        public static IEnumerable<ReleaseResource> GetUndeployedReleasesFromProjectPhase(OctopusRepository octRepository, ProjectResource project, string phaseName)
        {
            var releaseList = new List<ReleaseResource>();
            var environmentList = EnvironmentHelper.GetProjectEnvironments(octRepository, project, phaseName);
            var deployedReleases = GetDeployedReleasesFromProjectPhase(octRepository, project, phaseName);
            foreach (var environment in environmentList)
            {
                var deployedReleasesPerEnviornment = GetUndeployedReleasesFromProjectEnvironment(octRepository, project, environment);
                releaseList.AddRange(deployedReleasesPerEnviornment.Where(x => !releaseList.Any(d => d.GetVersionObject().Equals(x.GetVersionObject())) && !deployedReleases.Any(d => d.GetVersionObject().Equals(x.GetVersionObject()))));
            }
            return releaseList;
        }

        /// <summary>
        /// Gathers the latest release from a project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <returns>ReleaseResource</returns>
        public static ReleaseResource GetLatestReleaseFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            return GetProjectReleases(octRepository, project).OrderByDescending(r => r.GetVersionObject()).FirstOrDefault();
        }

        /// <summary>
        /// Update a Release's Package Step version with another version.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release"></param>
        /// <param name="packageStepDictionary"></param>
        public static void UpdateReleasePackageVersionByStep(OctopusRepository octRepository, ReleaseResource release, Dictionary<string, SemVersion> packageStepDictionary)
        {
            var selectedPackageList = release.SelectedPackages.ToDictionary(x => x.ActionName);
            foreach(var packageStep in packageStepDictionary)
            {
                selectedPackageList[packageStep.Key].Version = packageStep.Value.ToString();
            }
            release.SelectedPackages = selectedPackageList.Values.ToList();
            octRepository.Releases.Modify(release);
        }

        /// <summary>
        /// Gathers the last from the passed Project and Environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="environment">Environment to gather from.</param>
        /// <returns></returns>
        public static ReleaseResource GetLastestDeployedReleaseFromProjectEnvironmentByStatus(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment, string Status)
        {
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };

            var lastDeployedReleases = octRepository.Client.GetProjectEnvironmentDeployments(projectArray, environmentArray).ToList();
            
            if (lastDeployedReleases != null && lastDeployedReleases.Count > 0)
            {
                var lastDeployedReleaseId = lastDeployedReleases.OrderByDescending(p => p.Created).FirstOrDefault().ReleaseId;
                var lastDeployedRelease = octRepository.Releases.Get(lastDeployedReleaseId);
                return lastDeployedRelease;
            }
            else
            {
                return null;
            }
        }
    }
}