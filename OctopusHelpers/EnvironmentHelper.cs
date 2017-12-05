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
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="environmentName">Environment Name.</param>
        /// <returns>EnvironmentResource</returns>
        public static EnvironmentResource GetEnvironmentByName(OctopusRepository octRepository, string environmentName)
        {
            return octRepository.Environments.FindByName(environmentName);
        }

        /// <summary>
        /// Gathers a Environment by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="environmentId">Environment Id.</param>
        /// <returns>EnvironmentResource</returns>
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
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <returns>Enumerable of EnvironmentResources</returns>
        public static IEnumerable<EnvironmentResource> GetProjectEnvironments(OctopusRepository octRepository, ProjectResource project, string phaseName)
        {
            var lifecycle = LifecycleHelper.GetProjectLifeCycle(octRepository, project);
            return GetLifeCyclePhaseEnvironments(octRepository, lifecycle, phaseName);
        }

        /// <summary>
        /// Gathers a list of Environments of a Lifecycle
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="lifecycle">Lifecycle to gather from.</param>
        /// <param name="phaseName">Phase name to gather from</param>
        /// <returns>Enumerable of EnvironmentResources</returns>
        public static IEnumerable<EnvironmentResource> GetLifeCyclePhaseEnvironments(OctopusRepository octRepository, LifecycleResource lifecycle, string phaseName)
        {
            var environmentList = new List<EnvironmentResource>();
            var lifecylePhase = LifecycleHelper.GetPhaseByName(lifecycle, phaseName);
            return GetPhaseEnvironments(octRepository, lifecylePhase);
        }

        /// <summary>
        /// Gathers the list of Environments of a Phase
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="phase">Phase to gather from.</param>
        /// <returns>Enumerable of EnvironmentResources</returns>
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

        /// <summary>
        /// Creates an Environment
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="name">Name of the new Environment.</param>
        /// <param name="description">Description of the new Environment.</param>
        /// <param name="guidedFailure">Auto Enable GuidedFailure on Environment.</param>
        /// <returns>EnvironmentResource.</returns>
        public static EnvironmentResource CreateEnvironment (OctopusRepository octRepository, string name, string description, bool guidedFailure)
        {
            var environmentToCreate = new EnvironmentResource()
            {
                Name = name,
                UseGuidedFailure = guidedFailure,
                Description = description
            };

            return octRepository.Environments.Create(environmentToCreate);
        }
    }
}
