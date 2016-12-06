namespace OctopusHelpers.Constants
{
    /// <summary>
    /// All Error string needed for this library.
    /// </summary>
    internal class ErrorStrings
    {
        internal const string MissingPackageStep = "The Following Package Step did not have a corresponding key in the Dictionary Passed: {0}";
        internal const string MissingRequiredVar = "The Following Variable is Required and did not have a corresponding value in the Dictionary Passed: {0}";
        internal const string DeploymentAlreadyStarted = "The Deployment of: {0}, has already started.";
        internal const string DeploymentNotInCancellableState = "The Deployment of: {0} is not in a cancellable state, the state is curerntly: {1}.";
        internal const string DeploymentNotStarted = "The Deployment of: {0}, has not been started.";
    }
}
