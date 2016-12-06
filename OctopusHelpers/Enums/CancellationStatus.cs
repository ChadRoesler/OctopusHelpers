namespace OctopusHelpers.Enums
{
    /// <summary>
    /// Cancellation status Enum for OctopusDeploymentTaskManager.
    /// </summary>
    public enum CancellationStatus
    {
        None = 0,
        CancellationRequested = 1,
        CancellationSent = 2,
        Canceled = 3
    }
}