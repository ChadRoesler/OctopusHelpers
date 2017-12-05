using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helper for managing ActionTemplates Objects.
    /// </summary>
    public class ActionTemplateHelper
    {
        /// <summary>
        /// Gathers the Action Template Usage Info, Used for gathering what project a template may be tied to
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="actionTemplate"></param>
        /// <returns></returns>
        public IEnumerable<ActionTemplateUsageResource> GetActionTemplateUsage(OctopusRepository octRepository, ActionTemplateResource actionTemplate)
        {
            return octRepository.Client.GetActionTemplateUsage(actionTemplate);
        }
    }
}
