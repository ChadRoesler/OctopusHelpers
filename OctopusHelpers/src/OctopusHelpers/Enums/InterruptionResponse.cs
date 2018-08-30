namespace OctopusHelpers.Enums
{
    /// <summary>
    /// Interruption Response enum for managing an interruptiuon from an OctopusDeploymentTaskManager object.
    /// </summary>
    public enum InterruptionResponse
    {
        /// <summary>
        /// Retry the interruption.
        /// </summary>
        Retry = 0,
        /// <summary>
        /// Fail the deployment.
        /// </summary>
        Fail = 1,
        /// <summary>
        /// Cancel the deployment.
        /// </summary>
        Cancel = 2
    }
}
