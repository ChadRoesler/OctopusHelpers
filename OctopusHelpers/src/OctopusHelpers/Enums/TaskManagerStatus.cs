namespace OctopusHelpers.Enums
{
    /// <summary>
    /// TaskManagerStatus for gathering the status of a OctopusDeploymentTaskManager object.
    /// </summary>
    public enum TaskManagerStatus
    {
        /// <summary>
        /// Currently Executing.
        /// </summary>
        Executing = 0,
        /// <summary>
        /// Completed Task.
        /// </summary>
        Completed = 1,
        /// <summary>
        /// In the Process of Canceling.
        /// </summary>
        Canceling = 2,
        /// <summary>
        /// Canceled Task.
        /// </summary>
        Canceled = 3,
        /// <summary>
        /// Currently Interrupted and waiting for user input.
        /// </summary>
        Interrupted = 4,
        /// <summary>
        /// Queued Task.
        /// </summary>
        Queued = 5,
        /// <summary>
        /// The deployment has timed out.
        /// </summary>
        TimedOut =  6,
        /// <summary>
        /// Not started yet, but not queued.
        /// </summary>
        NotStarted = 7
    }
}
