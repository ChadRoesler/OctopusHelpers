using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing MachineResource Objects.
    /// </summary>
    public static class MachineHelper
    {
        /// <summary>
        /// Gathers a Machine by Id
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="machineId">Machine Id to gather</param>
        /// <returns>MachineResource</returns>
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

        /// <summary>
        /// Gathers machine by name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="machineName">Name of Machine to gather.</param>
        /// <returns>MachineResource</returns>
        public static MachineResource GetMachineByName(OctopusRepository octRepository, string machineName)
        {
            return octRepository.Machines.FindByName(machineName);
        }

        /// <summary>
        /// Adds a new Role to the machine.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="machine">Machine to add the role to.</param>
        /// <param name="roleName">Role to add.</param>
        public static void AddRoleToMachine (OctopusRepository octRepository, MachineResource machine, string roleName)
        {
            machine.Roles.Add(roleName);
            octRepository.Machines.Modify(machine);
        }

        /// <summary>
        /// Creates a machine Resources.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="name">Name of the new Machine.</param>
        /// <param name="roles">Roles to add to the Machine.</param>
        /// <param name="environmentIds">Environment Ids to add to the machine.</param>
        /// <param name="thumbprint">Thumbprint of the machine.</param>
        /// <param name="machineUri">Uri of the Machine.</param>
        /// <param name="machinePolicy">Policy of the Machine.</param>
        /// <returns>MachineResource</returns>
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

        /// <summary>
        /// Creates a machine Resources with the default policy.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="name">Name of the new Machine.</param>
        /// <param name="roles">Roles to add to the Machine.</param>
        /// <param name="environmentIds">Environment Ids to add to the machine.</param>
        /// <param name="thumbprint">Thumbprint of the machine.</param>
        /// <param name="machineUri">Uri of the Machine.</param>
        /// <returns>MachineResource</returns>
        public static MachineResource CreateMachineResource(OctopusRepository octRepository, string name, List<string> roles, List<string> environmentIds, string thumbprint, string machineUri)
        {
            var defaultMachinePolicy = MachinePolicyHelper.GetMachinePolicyByName(octRepository, ResourceStrings.DefaultMachinePolicyName);
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
