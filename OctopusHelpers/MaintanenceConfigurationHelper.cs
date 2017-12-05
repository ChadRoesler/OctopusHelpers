using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing MaintanenceConfiguration Objects.
    /// </summary>
    public static class MaintanenceConfigurationHelper
    {
        /// <summary>
        /// Sets the Maintanence Mode on the targeted octopus server
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="enable"></param>
        public static void SetMaintanenceMode(OctopusRepository octRepository, bool enable)
        {
            var currentMaintConfig = octRepository.Client.GetMaintenanceConfigurationResource();
            currentMaintConfig.IsInMaintenanceMode = enable;
            octRepository.Client.SetMaintenanceConfigurationResource(currentMaintConfig);
        }

        /// <summary>
        /// Gets the Maintenanace Configuration Object of the target Octopus Server
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <returns>MaintenanceConfigurationResource</returns>
        public static MaintenanceConfigurationResource GetMaintMode(OctopusRepository octRepository)
        {
            return octRepository.Client.GetMaintenanceConfigurationResource();
        }
    }
}
