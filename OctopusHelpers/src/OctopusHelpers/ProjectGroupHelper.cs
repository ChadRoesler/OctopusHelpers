using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing ProjectGroup Objects.
    /// </summary>
    public static class ProjectGroupHelper
    {
        /// <summary>
        /// Gathers a ProjectGroup by Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectGroupName"></param>
        /// <returns>ProjectGroupResource</returns>
        public static ProjectGroupResource GetProjectGroupByName(OctopusRepository octRepository, string projectGroupName)
        {
            return octRepository.ProjectGroups.FindByName(projectGroupName);
        }

        /// <summary>
        /// Gathers a ProjectGroup by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectGroupId"></param>
        /// <returns>ProjectGroupResource</returns>
        public static ProjectGroupResource GetProjectGroupById(OctopusRepository octRepository, string projectGroupId)
        {
            var numberOnly = new int();
            if (int.TryParse(projectGroupId, out numberOnly))
            {
                return octRepository.ProjectGroups.Get(string.Format(ResourceStrings.ProjectGroupIdFormat, projectGroupId));
            }
            else
            {
                return octRepository.ProjectGroups.Get(projectGroupId);
            }
        }

        /// <summary>
        /// Gathers a List of Projects in a ProjectGroup by Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectGroupName"></param>
        /// <returns>Enumerable of ProjectResources</returns>
        public static IEnumerable<ProjectResource> GetProjectsByProjectGroupName(OctopusRepository octRepository, string projectGroupName)
        {
            var projectGroup = GetProjectGroupByName(octRepository, projectGroupName);
            return octRepository.ProjectGroups.GetProjects(projectGroup);
        }

        /// <summary>
        /// Gathers a List of Projects in a ProjectGroup by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectGroupId"></param>
        /// <returns>Enumerable of ProjectResources</returns>
        public static IEnumerable<ProjectResource> GetProjectsByProjectGroupId(OctopusRepository octRepository, string projectGroupId)
        {
            var projectGroup = GetProjectGroupById(octRepository, projectGroupId);
            return octRepository.ProjectGroups.GetProjects(projectGroup);
        }

        /// <summary>
        /// Gathers a List of Projects in a ProjectGroup by ProjectGroup.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectGroup"></param>
        /// <returns>Enumerable of ProjectResources</returns>
        public static IEnumerable<ProjectResource> GetProjectsByProjectGroup(OctopusRepository octRepository, ProjectGroupResource projectGroup)
        {
            return octRepository.ProjectGroups.GetProjects(projectGroup);
        }

        /// <summary>
        /// Gathers all ProjectGroup Names.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <returns>Enumerable of all ProjectGroupNames</returns>
        public static IEnumerable<string> GetProjectGroupNames(OctopusRepository octRepository)
        {
            return octRepository.ProjectGroups.GetAll().Select(x => x.Name);
        }

        /// <summary>
        /// Moves a Project to the Specified ProjectGroup.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectToMove"></param>
        /// <param name="projectGroup"></param>
        public static void MoveProjectToProjectGroup(OctopusRepository octRepository, ProjectResource projectToMove, ProjectGroupResource projectGroup)
        {
            projectToMove.ProjectGroupId = projectGroup.Id;
            octRepository.Projects.Modify(projectToMove);
        }

        /// <summary>
        /// Creates a Project Group
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectGroupName">Name of the new Project Group.</param>
        /// <returns>Newly Created ProjectGroup</returns>
        public static ProjectGroupResource CreateProjectGroup(OctopusRepository octopusRepository, string projectGroupName)
        {
            var newProjectGroup = new ProjectGroupResource()
            {
                Name = projectGroupName
            };
            return octopusRepository.ProjectGroups.Create(newProjectGroup);
        }
    }
}
