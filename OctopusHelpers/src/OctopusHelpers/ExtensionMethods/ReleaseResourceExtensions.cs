using Semver;
using Octopus.Client.Model;

namespace OctopusHelpers.ExtensionMethods
{
    /// <summary>
    /// Release Resource Extensions.
    /// </summary>
    public static class ReleaseResourceExtensions
    {
        /// <summary>
        /// Retrieves the actual Version as a Version Object.
        /// </summary>
        /// <param name="currentReleaseResource">The release resource this is tacked on to.</param>
        /// <returns>Version object.</returns>
        public static SemVersion GetVersionObject(this ReleaseResource currentReleaseResource)
        {
            var versionObject = new SemVersion(0);
            SemVersion.TryParse(currentReleaseResource.Version, out versionObject);
            return versionObject;
        }
    }
}
