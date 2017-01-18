namespace OctopusHelpers.Enums
{
    /// <summary>
    /// TaskManagerStatus for gathering the status of a OctopusDeploymentTaskManager object.
    /// </summary>
    public enum TaskManagerStatus
    {
        /// <summary>
        ///
        /// </summary>
        Executing = 0,
        Completed = 1,
        Canceling = 2,
        Canceled = 3,
        Interrupted = 4,
        Queued = 5,
        NotStarted = 6
    }
}
