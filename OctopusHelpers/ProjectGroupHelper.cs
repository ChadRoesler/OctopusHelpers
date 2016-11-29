using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    public static class ProjectGroupHelper
    {
        /// <summary>
        /// Gathers a ProjectGroup by Name.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectGroupName"></param>
        /// <returns></returns>
        public static ProjectGroupResource GetProjectGroupByName(OctopusRepository octRepository, string projectGroupName)
        {
            return octRepository.ProjectGroups.FindByName(projectGroupName);
        }

        /// <summary>
        /// Gathers a ProjectGroup by Id.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectGroupId"></param>
        /// <returns></returns>
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
        /// <param name="octRepository"></param>
        /// <param name="projectGroupName"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectResource> GetProjectsByProjectGroupName(OctopusRepository octRepository, string projectGroupName)
        {
            var projectGroup = GetProjectGroupByName(octRepository, projectGroupName);
            return octRepository.Client.GetProjectGroupProjects(projectGroup);
        }

        /// <summary>
        /// Gathers a List of Projects in a ProjectGroup by Id.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectGroupId"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectResource> GetProjectsByProjectGroupId(OctopusRepository octRepository, string projectGroupId)
        {
            var projectGroup = GetProjectGroupById(octRepository, projectGroupId);
            return octRepository.Client.GetProjectGroupProjects(projectGroup);
        }

        /// <summary>
        /// Gathers a List of Projects in a ProjectGroup by ProjectGroup.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectGroup"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectResource> GetProjectsByProjectGroup(OctopusRepository octRepository, ProjectGroupResource projectGroup)
        {
            return octRepository.Client.GetProjectGroupProjects(projectGroup);
        }

        /// <summary>
        /// Gathers all ProjectGroup Names.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetProjectGroupNames(OctopusRepository octRepository)
        {
            return octRepository.ProjectGroups.GetAll().Select(x => x.Name);
        }

        /// <summary>
        /// Moves a Project to the Specified ProjectGroup.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="projectToMove"></param>
        /// <param name="projectGroup"></param>
        public static void MoveProjectToProjectGroup(OctopusRepository octRepository, ProjectResource projectToMove, ProjectGroupResource projectGroup)
        {
            projectToMove.ProjectGroupId = projectGroup.Id;
            octRepository.Projects.Modify(projectToMove);
        }
    }
}
