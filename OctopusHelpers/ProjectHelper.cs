using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Project Objects.
    /// </summary>
    public static class ProjectHelper
    {
        /// <summary>
        /// Gathers a Project By Name.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ProjectResource GetProjectByName(OctopusRepository octRepository, string projectName)
        {
            return octRepository.Projects.FindByName(projectName);
        }

        /// <summary>
        /// Gathers all Project Names.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetProjectNames(OctopusRepository octRepository)
        {
            return octRepository.Projects.GetAll().Select(x => x.Name);
        }

        /// <summary>
        /// Gathers a Project by Id.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static ProjectResource GetProjectById(OctopusRepository octRepository, string projectId)
        {
            var numberOnly = new int();
            if (int.TryParse(projectId, out numberOnly))
            {
                return octRepository.Projects.Get(string.Format(ResourceStrings.ProjectIdFormat, projectId));
            }
            else
            {
                return octRepository.Projects.Get(projectId);
            }
        }

        /// <summary>
        /// Creates a new Project from another Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="ProjectToCopy"></param>
        /// <param name="projectName"></param>
        /// <param name="projectDescription"></param>
        /// <param name="projectGroupToAddTo"></param>
        /// <returns></returns>
        public static ProjectResource CopyProjectFromProject(OctopusRepository octRepository, ProjectResource ProjectToCopy, string projectName, string projectDescription, ProjectGroupResource projectGroupToAddTo = null, LifecycleResource lifecycleToAdd = null)
        {
            octRepository.Client.CloneProject(projectName, projectDescription, ProjectToCopy, projectGroupToAddTo.Id, lifecycleToAdd.Id);
            return GetProjectByName(octRepository, projectName);
        }

        /// <summary>
        /// Deletes the specified Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectToDelete"></param>
        public static void DeleteProject(OctopusRepository octRepository, ProjectResource projectToDelete)
        {
            octRepository.Projects.Delete(projectToDelete);
        }

    }
}
