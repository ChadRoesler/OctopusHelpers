using System;
using Octopus.Client.Model;

namespace OctopusHelpers.ExtensionMethods
{
    public static class ReleaseResourceExtensions
    {
        /// <summary>
        /// Retrives the actuall Version as a Version Object.
        /// </summary>
        /// <param name="currentReleaseResource"></param>
        /// <returns></returns>
        public static Version GetVersionObject(this ReleaseResource currentReleaseResource)
        {
            var versionObject = new Version();
            Version.TryParse(currentReleaseResource.Version, out versionObject);
            return versionObject;
        }
    }
}
