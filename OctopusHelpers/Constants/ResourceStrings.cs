namespace OctopusHelpers.Constants
{
    /// <summary>
    /// All Resource Strings needed for this library.
    /// </summary>
    internal class ResourceStrings
    {
        internal const string EnvironmentIdFormat = "environments-{0}";
        internal const string ProjectIdFormat = "projects-{0}";
        internal const string ProjectGroupIdFormat = "ProjectGroups-{0}";
        internal const string LibraryVariableSetIdFormat = "LibraryVariableSets-{0}";
        internal const string UserIdFormat = "users-{0}";
        internal const string LifecycleIdFormat = "Lifecycles-{0}";
        internal const string TeamIdFormat = "Teams-{0}";
        internal const string CloneCommandApiFormat = "~/api/projects?clone={0}";
        internal const string MaintenanceConfigApi = "~/api/maintenanceconfiguration";
        internal const string VarsetVars = "~/api/variables/{0}";
        internal const string TeamUserIdFormat = "~/api/Users/{0}";
        internal const string ProjectsLink = "Projects";
        internal const string QueuedBehindLink = "QueuedBehind";
        internal const string ScriptModuleNameFormat = "Octopus.Script.Module[{0}]";
        internal const string PackageActionType = "Octopus.TentaclePackage";
        internal const string DummyPackageVersion = "0.0.0.0";
        internal const string DummyReleaseVersion = "0.0.0.0";
        internal const string SelfLink = "Self";
        internal const string OctopusDeploymentLink = "/app#/deployments/{0}";

        internal const string InterruptionGuidanceKey = "Guidance";
        internal const string InterruptionRetryValue = "Retry";
        internal const string InterruptionFailValue = "Fail";
        internal const string InterruptionNoteKey = "Notes";

        internal const string Return = "/r/n";
        internal const string ErrorPrinting = "{0}{1}";

        internal const string MetaStepName = "MetaStep";
        internal const string DeploymentIdKey = "DeploymentId";
    }
}
