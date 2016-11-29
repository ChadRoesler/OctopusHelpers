using System.Collections.Generic;
using OctopusHelpers.Constants;
using Octopus.Client;
using Octopus.Client.Model;

namespace OctopusHelpers
{
    public static class EnvironmentHelper
    {
        /// <summary>
        /// Gathers a Environment by Name.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="environmentName"></param>
        /// <returns></returns>
        public static EnvironmentResource GetEnvironmentByName(OctopusRepository octRepository, string environmentName)
        {
            return octRepository.Environments.FindByName(environmentName);
        }

        /// <summary>
        /// Gathers a Environment by Id.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        public static EnvironmentResource GetEnvironmentById(OctopusRepository octRepository, string environmentId)
        {
            var numberOnly = new int();
            if (int.TryParse(environmentId, out numberOnly))
            {
                return octRepository.Environments.Get(string.Format(ResourceStrings.EnvironmentIdFormat, environmentId));
            }
            else
            {
                return octRepository.Environments.Get(environmentId);
            }
        }

        /// <summary>
        /// Gathers a list Environments of a Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<EnvironmentResource> GetProjectEnvironments(OctopusRepository octRepository, ProjectResource project)
        {
            var projectGroup = ProjectGroupHelper.GetProjectGroupById(octRepository, project.ProjectGroupId);
            var environmentList = new List<EnvironmentResource>();
            foreach (var environment in projectGroup.EnvironmentIds)
            {
                var environmentToAdd = GetEnvironmentById(octRepository, environment);
                environmentList.Add(environmentToAdd);
            }
            return environmentList;
        }
    }
}
