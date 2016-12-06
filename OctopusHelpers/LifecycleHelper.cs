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
        /// <param name="octRepository"></param>
        /// <param name="lifecycleId"></param>
        /// <returns></returns>
        public static LifecycleResource GetLifecycleById (OctopusRepository octRepository, string lifecycleId)
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
        /// <param name="octRepository"></param>
        /// <param name="lifecycleName"></param>
        /// <returns></returns>
        public static LifecycleResource GetLifecycleByName(OctopusRepository octRepository, string lifecycleName)
        {
            return octRepository.Lifecycles.FindByName(lifecycleName);
        }

        /// <summary>
        /// Gathers the Lifecycle from a project
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static LifecycleResource GetProjectLifeCycle(OctopusRepository octRepository, ProjectResource project)
        {
            return octRepository.Lifecycles.Get(project.LifecycleId);
        }

        /// <summary>
        /// Returns the Phase of a Lifecycle by Name.
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <param name="phaseName"></param>
        /// <returns></returns>
        public static PhaseResource GetPhaseByName(LifecycleResource lifecycle, string phaseName)
        {
            return lifecycle.Phases.Where(x => x.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        
    }
}
