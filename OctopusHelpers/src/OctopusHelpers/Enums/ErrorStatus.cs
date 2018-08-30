namespace OctopusHelpers.Enums
{
    /// <summary>
    /// Error status enums for an OctopusDeploymentTaskManager object.
    /// </summary>
    public enum ErrorStatus
    {
        /// <summary>
        /// No errors returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// Warnings returned.
        /// </summary>
        Warnings = 1,
        /// <summary>
        /// Errors returned.
        /// </summary>
        Error = 2
    }
}
