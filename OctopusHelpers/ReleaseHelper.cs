using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    public static class ReleaseHelper
    {
        /// <summary>
        /// Get all releases of a Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<ReleaseResource> GetProjectReleases(OctopusRepository octRepository, ProjectResource project)
        {
            var releaseList = new List<ReleaseResource>();
            var projectReleases = octRepository.Projects.GetReleases(project);
            if (projectReleases != null && projectReleases.TotalResults > 0)
            {
                releaseList.AddRange(projectReleases.Items.ToList());
            }
            return releaseList;
        }

        /// <summary>
        /// Gather a Project's Release from the passed Version.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="releaseVersion"></param>
        /// <returns></returns>
        public static ReleaseResource GetProjectReleaseByVersion(OctopusRepository octRepository, ProjectResource project, Version releaseVersion)
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
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="releaseVersion"></param>
        /// <param name="stepAndVersionDictionary"></param>
        /// <param name="releaseNotes"></param>
        public static void CreateProjectRelease(OctopusRepository octRepository, ProjectResource project, string releaseVersion, Dictionary<string, Version> stepAndVersionDictionary, string releaseNotes)
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
            octRepository.Releases.Create(release);
        }

        /// <summary>
        /// Creates a dummy release and deletes it on the passed Project for deployment process updating.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
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
        /// <param name="octRepository"></param>
        /// <param name="release"></param>
        public static void DeleteRelease(OctopusRepository octRepository, ReleaseResource release)
        {
            octRepository.Releases.Delete(release);
        }

        /// <summary>
        /// Updates the Variables for the passed Release.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="release"></param>
        public static void UpdateReleaseVariables(OctopusRepository octRepository, ReleaseResource release)
        {
            octRepository.Releases.SnapshotVariables(release);
        }

        /// <summary>
        /// Gathers a list of deployed Releases for a passed Project and Environment. (This has to be done by ReleaseId due to not having anyother way to connect deployed releases to release resources).
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IEnumerable<ReleaseResource> GetDeployedReleasesFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var releaseList = new List<ReleaseResource>();
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var deployments = octRepository.Deployments.FindAll(projectArray, environmentArray, 0);
            if (deployments != null && deployments.TotalResults > 0)
            {
                var deploymentItems = deployments.Items;
                var releases = GetProjectReleases(octRepository, project);
                if (releases != null && releases.Count() > 0)
                {
                    releaseList.AddRange(releases.Where(x => deploymentItems.Any(d => d.ReleaseId.Equals(x.Id))).ToList());
                }
            }
            return releaseList;
        }

        /// <summary>
        /// Gathers a list of last deployed releases per environment.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<ReleaseResource> GetLastDeployedReleasesFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var releaseList = new List<ReleaseResource>();
            var environmentList = EnvironmentHelper.GetProjectEnvironments(octRepository, project);
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
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static ReleaseResource GetLastDeployedReleaseFromProjectEnvironment(OctopusRepository octRepository, ProjectResource project, EnvironmentResource environment)
        {
            var projectArray = new string[] { project.Id };
            var environmentArray = new string[] { environment.Id };
            var lastDeployedReleases = octRepository.Deployments.FindAll(projectArray, environmentArray, 0);
            if (lastDeployedReleases != null && lastDeployedReleases.TotalResults > 0)
            {
                var lastDeployedReleaseId = lastDeployedReleases.Items.OrderByDescending(p => p.Created).FirstOrDefault().ReleaseId;
                var lastDeployedRelease = octRepository.Releases.Get(lastDeployedReleaseId);
                return lastDeployedRelease;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gathers a list of undeployed Releases from the passed Project and Environment.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
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
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<ReleaseResource> GetDeployedReleasesFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var releaseList = new List<ReleaseResource>();
            var environmentList = EnvironmentHelper.GetProjectEnvironments(octRepository, project);
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
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<ReleaseResource> GetUndeployedReleasesFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var releaseList = new List<ReleaseResource>();
            var environmentList = EnvironmentHelper.GetProjectEnvironments(octRepository, project);
            var deployedReleases = GetDeployedReleasesFromProject(octRepository, project);
            foreach (var environment in environmentList)
            {
                var deployedReleasesPerEnviornment = GetUndeployedReleasesFromProjectEnvironment(octRepository, project, environment);
                releaseList.AddRange(deployedReleasesPerEnviornment.Where(x => !releaseList.Any(d => d.GetVersionObject().Equals(x.GetVersionObject())) && !deployedReleases.Any(d => d.GetVersionObject().Equals(x.GetVersionObject()))));
            }
            return releaseList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static ReleaseResource GetLatestReleaseFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            return GetProjectReleases(octRepository, project).OrderByDescending(r => r.GetVersionObject()).FirstOrDefault();
        }

        /// <summary>
        /// Update a Release's Package Step version with another version.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="release"></param>
        /// <param name="packageStepDictionary"></param>
        public static void UpdateReleasePackageVersionByStep(OctopusRepository octRepository, ReleaseResource release, Dictionary<string, Version> packageStepDictionary)
        {
            var selectedPackageList = release.SelectedPackages.ToDictionary(x => x.StepName);
            foreach(var packageStep in packageStepDictionary)
            {
                selectedPackageList[packageStep.Key].Version = packageStep.Value.ToString();
            }
            release.SelectedPackages = selectedPackageList.Values.ToList();
            octRepository.Releases.Modify(release);
        }
    }
}