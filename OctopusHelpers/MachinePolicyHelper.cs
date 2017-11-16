using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class MachinePolicyHelper
    {
        public static MachinePolicyResource GetMachineById(OctopusRepository octRepository, string machinePolicyId)
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

        public static MachinePolicyResource GetMachineByName(OctopusRepository octRepository, string machinePolicyName)
        {
            return octRepository.MachinePolicies.FindByName(machinePolicyName);
        }
    }
}
