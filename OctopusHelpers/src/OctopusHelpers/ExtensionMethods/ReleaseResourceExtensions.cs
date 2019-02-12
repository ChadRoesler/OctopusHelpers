using System;
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
        public static SemVersion GetSemVersionObject(this ReleaseResource currentReleaseResource)
        {
            var semVersionObject = new SemVersion(0);
            SemVersion.TryParse(currentReleaseResource.Version, out semVersionObject);
            return semVersionObject;
        }

        /// <summary>
        /// Allows custom transformation of ver to sem ver
        /// usefull in situations were you have a part ver and want it appropriate
        /// Example: 0.0.5.1 to sem ver is 0.0.1+5
        /// wanting it to be semv
        /// </summary>
        /// <param name="currentReleaseResource">he release resource this is tacked on to.</param>
        /// <returns>SemanticVersion Object</returns>
        public static SemanticVersion GetSemanticVerionObject(this ReleaseResource currentReleaseResource)
        {
            var semanticVersionObject = new SemanticVersion("0.0.0.0");
            SemanticVersion.TryParse(currentReleaseResource.Version, out semanticVersionObject);
            return semanticVersionObject;
        }
        

        /// <summary>
        /// Retrieves the actual Version as a Version Object.
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
