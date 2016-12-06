using System.Collections.Generic;
using OctopusHelpers.Constants;
using Octopus.Client;
using Octopus.Client.Model;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Environment Objects.
    /// </summary>
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
        public static IEnumerable<EnvironmentResource> GetProjectEnvironments(OctopusRepository octRepository, ProjectResource project, string phaseName)
        {
            var lifecycle = LifecycleHelper.GetProjectLifeCycle(octRepository, project);
            return GetLifeCyclePhaseEnvironments(octRepository, lifecycle, phaseName);
        }

        /// <summary>
        /// Gathers a list of Environments of a Lifecycle
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="lifecycle"></param>
        /// <param name="phaseName"></param>
        /// <returns></returns>
        public static IEnumerable<EnvironmentResource> GetLifeCyclePhaseEnvironments(OctopusRepository octRepository, LifecycleResource lifecycle, string phaseName)
        {
            var environmentList = new List<EnvironmentResource>();
            var lifecylePhase = LifecycleHelper.GetPhaseByName(lifecycle, phaseName);
            return GetPhaseEnvironments(octRepository, lifecylePhase);
        }

        /// <summary>
        /// Gathers the list of Environments of a Phase
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        public static IEnumerable<EnvironmentResource> GetPhaseEnvironments(OctopusRepository octRepository, PhaseResource phase)
        {
            var environmentList = new List<EnvironmentResource>();
            foreach (var environment in phase.OptionalDeploymentTargets)
            {
                var environmentToAdd = GetEnvironmentById(octRepository, environment);
                environmentList.Add(environmentToAdd);
            }
            return environmentList;
        }
    }
}
