using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    public class ActionTemplateHelper
    {
        public IEnumerable<ActionTemplateUsageResource> GetActionTemplateUsage(OctopusRepository octRepository, ActionTemplateResource actionTemplate)
        {
            return octRepository.Client.GetActionTemplateUsage(actionTemplate);
        }
    }
}
