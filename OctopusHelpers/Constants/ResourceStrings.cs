namespace OctopusHelpers.Constants
{
	/// <summary>
	/// Different strings needed for various helpers.
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
		internal const string MachineIdFormat = "Machines-{0}";
		internal const string TaskIdFormat = "ServerTasks-{0}";
		internal const string CloneCommandApiFormat = "~/api/projects?clone={0}";
		internal const string MaintenanceConfigApi = "~/api/maintenanceconfiguration";
		internal const string EventRegardingLink = "{0}?regarding={1}&eventCategories={2}";
		internal const string InterruptionRegardingLink = "{0}?regarding={1}&pendingOnly={2}";
		internal const string VarsetVars = "~/api/variables/{0}";
		internal const string TeamUserIdFormat = "~/api/Users/{0}";
		internal const string QueuedBehindLink = "QueuedBehind";
		internal const string ReleaseLink = "Releases";
		internal const string ScriptModuleNameFormat = "Octopus.Script.Module[{0}]";
		internal const string PackageActionType = "Octopus.TentaclePackage";
		internal const string DummyPackageVersion = "0.0.0.0";
		internal const string DummyReleaseVersion = "0.0.0.0";
		internal const string SelfLink = "Self";
		internal const string EventLink = "Events";
        internal const string UsageLink = "Usage";
		internal const string InterruptionLink = "Interruptions";
		internal const string OctopusDeploymentLink = "/app#/deployments/{0}";
		internal const string CancelledTaskEventCategory = "TaskCanceled";
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

		/// <summary>
		/// Abandon all hope ye who enter here.
		/// This for backdoor modification for updating deployment processes.
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

        internal const string RegExFormatPatternVariableValueBegin = @"\#\{\b";
        internal const string RegExFormatPatternVariableValueEnd = @"\b\}";
        internal const string RegExFormatPatterScriptModuleBegin = @"\$\bOctopusParameters\b\[\""\b";
        internal const string RegExFormatPatterScriptModuleEnd = @"\b\""\]";

    }
}
