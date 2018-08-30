using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing MachinePolicy Objects.
    /// </summary>
    public static class MachinePolicyHelper
    {
        /// <summary>
        /// Gathers MachinePolicy By Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="machinePolicyId">Id of the machine Policy</param>
        /// <returns>MachinePolicyResource</returns>
        public static MachinePolicyResource GetMachinePolicyById(OctopusRepository octRepository, string machinePolicyId)
        {
            var numberOnly = new int();
            if (int.TryParse(machinePolicyId, out numberOnly))
            {
                return octRepository.MachinePolicies.Get(string.Format(ResourceStrings.MachinePolicyIdFormat, machinePolicyId));
            }
            else
            {
                return octRepository.MachinePolicies.Get(machinePolicyId);
            }
        }

        /// <summary>
        /// Gathers MachinePolicy By name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="machinePolicyName">Name of the machine Policy.</param>
        /// <returns>MachinePolicyResource</returns>
        public static MachinePolicyResource GetMachinePolicyByName(OctopusRepository octRepository, string machinePolicyName)
        {
            return octRepository.MachinePolicies.FindByName(machinePolicyName);
        }
    }
}
