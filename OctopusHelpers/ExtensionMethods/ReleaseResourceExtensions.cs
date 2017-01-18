using System;
using Octopus.Client.Model;

namespace OctopusHelpers.ExtensionMethods
{
    /// <summary>
    /// Release Resourec Extentions.
    /// </summary>
    public static class ReleaseResourceExtensions
    {
        /// <summary>
        /// Retrives the actuall Version as a Version Object.
        /// </summary>
        /// <param name="currentReleaseResource">The release resource this is tacked on to.</param>
        /// <returns>Version object.</returns>
        public static Version GetVersionObject(this ReleaseResource currentReleaseResource)
        {
            var versionObject = new Version();
            Version.TryParse(currentReleaseResource.Version, out versionObject);
            return versionObject;
        }
    }
}
