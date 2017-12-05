namespace OctopusHelpers.Enums
{
    /// <summary>
    /// Cancellation status enum for an OctopusDeploymentTaskManager object.
    /// </summary>
    public enum CancellationStatus
    {
        /// <summary>
        /// No cancellation requested.
        /// </summary>
        None = 0,
        /// <summary>
        /// Cancellation Requested for managing.
        /// </summary>
        CancellationRequested = 1,
        /// <summary>
        /// Cancellation sent to octopus.
        /// </summary>
        CancellationSent = 2,
        /// <summary>
        /// Canceled in octopus.
        /// </summary>
        Canceled = 3
    }
}