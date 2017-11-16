using System.Collections.Generic;
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

        public static void AddRoleToMachine (OctopusRepository octRepository, MachineResource machine, string roleName)
        {
            machine.Roles.Add(roleName);
            octRepository.Machines.Modify(machine);
        }

        public static MachineResource CreateMachineResource(OctopusRepository octRepository, string name, List<string> roles, List<string> environmentIds, string thumbprint, string machineUri, MachinePolicyResource machinePolicy)
        {

            var rolesReferenceCollection = new ReferenceCollection(roles);
            var evironmentReferenceCollection = new ReferenceCollection(environmentIds);
            var machineToCreate = new MachineResource()
            {
                Name = name,
                Roles = rolesReferenceCollection,
                EnvironmentIds = evironmentReferenceCollection,
                Thumbprint = thumbprint,
                MachinePolicyId = machinePolicy.Id,
                Uri = machineUri
            };

            return octRepository.Machines.Create(machineToCreate);
        }

        public static MachineResource CreateMachineResource(OctopusRepository octRepository, string name, List<string> roles, List<string> environmentIds, string thumbprint, string machineUri)
        {
            var defaultMachinePolicy = MachinePolicyHelper.GetMachineByName(octRepository, ResourceStrings.DefaultMachinePolicyName);
            var rolesReferenceCollection = new ReferenceCollection(roles);
            var evironmentReferenceCollection = new ReferenceCollection(environmentIds);
            var machineToCreate = new MachineResource()
            {
                Name = name,
                Roles = rolesReferenceCollection,
                EnvironmentIds = evironmentReferenceCollection,
                Thumbprint = thumbprint,
                Uri = machineUri,
                MachinePolicyId = defaultMachinePolicy.Id
            };
            return octRepository.Machines.Create(machineToCreate);
        }
    }
}
