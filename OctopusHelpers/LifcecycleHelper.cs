using System;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing LifeCycle Objects.
    /// </summary>
    public static class LifecycleHelper
    {
        /// <summary>
        /// Gathers a Lifecycle by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="lifecycleId">Id of the Lifecycke.</param>
        /// <returns>LifecycleResource</returns>
        public static LifecycleResource GetLifecycleById(OctopusRepository octRepository, string lifecycleId)
        {
            var numberOnly = new int();
            if (int.TryParse(lifecycleId, out numberOnly))
            {
                return octRepository.Lifecycles.Get(string.Format(ResourceStrings.LifecycleIdFormat, lifecycleId));
            }
            else
            {
                return octRepository.Lifecycles.Get(lifecycleId);
            }
        }

        /// <summary>
        /// Gathers a Lifecycle by Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="lifecycleName">Name of the Lifecycle.</param>
        /// <returns>LifecycleResource</returns>
        public static LifecycleResource GetLifecycleByName(OctopusRepository octRepository, string lifecycleName)
        {
            return octRepository.Lifecycles.FindByName(lifecycleName);
        }

        /// <summary>
        /// Gathers the Lifecycle from a project
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <returns>LifecycleResource</returns>
        public static LifecycleResource GetProjectLifeCycle(OctopusRepository octRepository, ProjectResource project)
        {
            return octRepository.Lifecycles.Get(project.LifecycleId);
        }

        /// <summary>
        /// Returns the Phase of a Lifecycle by Name.
        /// </summary>
        /// <param name="lifecycle">Lifecycle to gather phase from.</param>
        /// <param name="phaseName">Phase Name to gather.</param>
        /// <returns>PhaseResource</returns>
        public static PhaseResource GetPhaseByName(LifecycleResource lifecycle, string phaseName)
        {
            return lifecycle.Phases.Where(x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}