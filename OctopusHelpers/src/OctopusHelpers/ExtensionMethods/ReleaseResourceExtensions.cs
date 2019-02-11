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
        /// <param name="semVerFormat">Version format for Semver: {Major}.{Minor}.{Build}+{Revision}</param>
        /// <returns></returns>
        public static SemVersion GetSemVersionObject(this ReleaseResource currentReleaseResource, string semVerFormat)
        {
            var semVersionObject = new SemVersion(0);
            var releaseVersionObject = currentReleaseResource.GetVersionObject();
            semVerFormat = semVerFormat.Replace("{Major}", releaseVersionObject.Major.ToString()).Replace("{Minor}", releaseVersionObject.Minor.ToString()).Replace("{Build}", releaseVersionObject.Build.ToString()).Replace("{Revision}", releaseVersionObject.Revision.ToString());
            SemVersion.TryParse(semVerFormat, out semVersionObject);
            return semVersionObject;
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
