using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class MachineHelper
    {
        public static MachineResource GetMachineById(OctopusRepository octRepository, string machineId)
        {
            var numberOnly = new int();
            if (int.TryParse(machineId, out numberOnly))
            {
                return octRepository.Machines.Get(string.Format(ResourceStrings.MachineIdFormat, machineId));
            }
            else
            {
                return octRepository.Machines.Get(machineId);
            }
        }

        public static MachineResource GetMachineByName(OctopusRepository octRepository, string machineName)
        {
            return octRepository.Machines.FindByName(machineName);
        }
    }
}
