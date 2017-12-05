namespace OctopusHelpers.Constants
{
	/// <summary>
	/// Different strings needed for various helpers.
	/// </summary>
	internal class ResourceStrings
	{
		internal const string EnvironmentIdFormat = "environments-{0}";
		internal const string ProjectIdFormat = "projects-{0}";
		internal const string ProjectGroupIdFormat = "projectgroups-{0}";
		internal const string LibraryVariableSetIdFormat = "libraryvariableSets-{0}";
		internal const string UserIdFormat = "users-{0}";
        internal const string ChannelIdFormat = "channel-{0}";
		internal const string LifecycleIdFormat = "lifecycles-{0}";
		internal const string TeamIdFormat = "teams-{0}";
		internal const string MachineIdFormat = "machines-{0}";
		internal const string TaskIdFormat = "serverTasks-{0}";
        internal const string MachinePolicyIdFormat = "machinepolicies-{0}";
        internal const string ProjectNameApiFormat = "~/api/projects?name={0}";
		internal const string MaintenanceConfigApi = "~/api/maintenanceconfiguration";
		internal const string EventRegardingLink = "{0}?regarding={1}&eventCategories={2}";
		internal const string InterruptionRegardingLink = "{0}?regarding={1}&pendingOnly={2}";
        internal const string DeploymentsLink = "{0}?projects={1}&environments={2}";
        internal const string TeamUserIdFormat = "~/api/Users/{0}";
		internal const string ScriptModuleNameFormat = "Octopus.Script.Module[{0}]";
		internal const string PackageActionType = "Octopus.TentaclePackage";
		internal const string DummyPackageVersion = "0.0.0.0";
		internal const string DummyReleaseVersion = "0.0.0.0";
        internal const string ReleaseLink = "releases";
        internal const string SelfLink = "self";
		internal const string EventLink = "events";
		internal const string UsageLink = "usage";
        internal const string ChannelLink = "channels";
        internal const string DeploymentLink = "deployments";
		internal const string InterruptionLink = "interruptions";
		internal const string OctopusDeploymentLink = "/app#/deployments/{0}";
		internal const string CancelledTaskEventCategory = "TaskCanceled";
        internal const string DefaultMachinePolicyName = "Default Machine Policy";
        internal const string FormValuesArgException = "formValues";
		internal const string InterruptionGuidanceKey = "Guidance";
		internal const string InterruptionRetryValue = "Retry";
		internal const string InterruptionFailValue = "Fail";
		internal const string InterruptionNoteKey = "Notes";
		internal const string Return = @"
";
		internal const string LogPrinting = "{0}{1}";
		internal const string MetaStepName = "MetaStep";
		internal const string DeploymentIdKey = "DeploymentId";

		internal const string RegExFormatPatternVariableValueBegin = @"\#\{\b";
		internal const string RegExFormatPatternVariableValueEnd = @"\b\}";
		internal const string RegExFormatPatterScriptModuleBegin = @"\$\bOctopusParameters\b\[[\""\']\b";

		internal const string RegExFormatPatterScriptModuleEnd = @"\b[\""\']\]";
		internal const string ScripPropertyType = "Script";
		internal const string ScriptModuleNameReplacement = "Octopus.Script.Module[";

        /// <summary>
        /// Abandon all hope ye who enter here.
        /// This for back-door modification for updating deployment processes.
        /// http://i0.kym-cdn.com/photos/images/newsfeed/001/040/082/cb4.png
        /// </summary>
        internal const string ProjectProcessUpdate = @"UPDATE	dp
SET		dp.JSON = o.JSON
FROM	DeploymentProcess dp
		INNER JOIN Project p ON p.Id = dp.OwnerId
								AND p.DeploymentProcessId <> dp.Id
		INNER JOIN (SELECT	dp.JSON,
							p.Id
					FROM	Project p
							INNER JOIN DeploymentProcess dp on dp.Id = p.DeploymentProcessId) o ON o.Id = dp.OwnerId
WHERE p.Id = @ProjectId";
		internal const string ProjectIdParameter = "@ProjectId";
		internal const string ParameterPairings = "Parameter: {0}, Value: {1}";
	}
}
