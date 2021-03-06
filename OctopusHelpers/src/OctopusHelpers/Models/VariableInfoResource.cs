﻿using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers.Models
{
    /// <summary>
    /// Look some time this might be a real thing, i dont wanna you cant make me.
    /// </summary>
    public class VariableInfoResource
    {
        /// <Note>
        /// Further work will be done here to allow omitting and altering of variables this allow for storage for now.
        /// OR this needs to be split out into its own area and not as a model.
        /// I don't know yet on this, but currently leaving it as a model as there is a specific reason for having this, exporting data to excel.
        /// I'll be writing documentation for this later, if it still exists
        /// </Note>
        private List<VariableResource> VariableList = new List<VariableResource>();
        private List<VariableResource> ScriptModuleList = new List<VariableResource>();
        private List<string> FileList = new List<string>();
        private List<ProjectResource> ProjectScriptList = new List<ProjectResource>();
        private List<ActionTemplateResource> StepTemplateScriptList = new List<ActionTemplateResource>();
        private List<ProjectResource> ProjectVariablesList = new List<ProjectResource>();
        private List<ActionTemplateResource> StepTemplateVariablesList = new List<ActionTemplateResource>();

        private List<EnvironmentResource> EnvironmentScope = new List<EnvironmentResource>();
        private List<MachineResource> MachineScope = new List<MachineResource>();
        private List<string> RoleScope = new List<string>();
        private LibraryVariableSetResource LibraryVariableSet;
        private VariableResource Variable;
        private OctopusRepository OctopusRepo;

        /// <summary>
        /// Gonna go out for a pack of smokes.
        /// </summary>
        /// <param name="octRepository">Ill be</param>
        /// <param name="variable">back</param>
        /// <param name="library">soon</param>
        public VariableInfoResource(OctopusRepository octRepository, VariableResource variable, LibraryVariableSetResource library)
        {
            Variable = variable;
            LibraryVariableSet = library;
            LibrarySetName = library.Name;
            VariableSetId = library.VariableSetId;
            LibrarySetId = library.Id;
            OctopusRepo = octRepository;

            Id = variable.Id;
            Name = variable.Name;
            Sensative = variable.IsSensitive;
            Value = variable.Value;
            DeploymentTimeValue = variable.Value;

            if (variable.Scope.Count > 0)
            {
                if (variable.Scope.ContainsKey(ScopeField.Environment))
                {
                    foreach (var environment in variable.Scope[ScopeField.Environment])
                    {
                        EnvironmentScope.Add(EnvironmentHelper.GetEnvironmentById(OctopusRepo, environment));
                    }
                }
                if (variable.Scope.ContainsKey(ScopeField.Role))
                {
                    RoleScope = variable.Scope[ScopeField.Role].ToList();
                }
                if (variable.Scope.ContainsKey(ScopeField.Machine))
                {
                    foreach (var machine in variable.Scope[ScopeField.Machine])
                    {
                        MachineScope.Add(MachineHelper.GetMachineById(OctopusRepo, machine));
                    }
                }
            }
        }

        public string LibrarySetName { get; set; }
        public string LibrarySetId { get; set; }
        public string VariableSetId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DeploymentTimeValue { get; set; }
        public bool Sensative { get; set; }

        public List<VariableResource> UsedInVariableList
        {
            get { return VariableList; }
        }
        public List<VariableResource> UsedInScriptModuleList
        {
            get { return ScriptModuleList; }
        }
        public List<string> UsedInFileList
        {
            get { return FileList; }
        }
        public List<ProjectResource> UsedInProjectScriptList
        {
            get { return ProjectScriptList; }
        }
        public List<ActionTemplateResource> UsedInStepTemplateScriptList
        {
            get { return StepTemplateScriptList; }
        }
        public List<ProjectResource> UsedInProjectVariablesList
        {
            get { return ProjectVariablesList; }
        }
        public List<ActionTemplateResource> UsedInStepTemplateVariablesList
        {
            get { return StepTemplateVariablesList; }
        }

        public string EnvironmentScopeNames
        {
            get
            {
                if (EnvironmentScope.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", EnvironmentScope.Select(x => x.Name));
                }
            }
        }
        public string RoleScopeNames
        {
            get
            {
                if (RoleScope.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", RoleScope);
                }
            }
        }
        public string MachineScopeNames
        {
            get
            {
                if (MachineScope.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", MachineScope.Select(x => x.Name).ToList());
                }
            }
        }
        public string UsedInVariableNames
        {
            get
            {
                if (UsedInVariableList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInVariableList.Select(x => x.Name));
                }
            }
        }
        public string UsedInScriptModuleNames
        {
            get
            {
                if (UsedInScriptModuleList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInScriptModuleList.Select(x => x.Name.Replace(ResourceStrings.ScriptModuleNameReplacement, string.Empty).TrimEnd(']')).ToList());
                }
            }
        }
        public string UsedInFileNames
        {
            get
            {
                if (UsedInFileList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInFileList);
                }
            }
        }
        public string UsedInProjectScriptNames
        {
            get
            {
                if (UsedInProjectScriptList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInProjectScriptList.Select(x => x.Name));
                }
            }
        }
        public string UsedInStepTemplateScriptNames
        {
            get
            {
                if (UsedInStepTemplateScriptList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInStepTemplateScriptList.Select(x => x.Name));
                }
            }
        }
        public string UsedInProjectVariableNames
        {
            get
            {
                if (UsedInProjectVariablesList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInProjectVariablesList.Select(x => x.Name));
                }
            }
        }
        public string UsedInStepTemplateVariableNames
        {
            get
            {
                if (UsedInStepTemplateVariablesList.Count() == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", UsedInStepTemplateVariablesList.Select(x => x.Name));
                }
            }
        }

        public bool Unused
        {
            get
            {
                return (VariableList.Count == 0 && ScriptModuleList.Count == 0 && FileList.Count == 0 && ProjectScriptList.Count == 0 && StepTemplateScriptList.Count == 0 && StepTemplateVariablesList.Count == 0 && ProjectVariablesList.Count == 0);
            }
        }

        public void LoadReferencedVariables(List<VariableResource> variableList)
        {

            foreach (var variable in variableList)
            {
                if (!string.IsNullOrWhiteSpace(variable.Value) && VariableRegExHelper.VarTextContainsVariable(variable.Value, Name))
                {
                    VariableList.Add(variable);
                }
            }
        }

        public void LoadDeployTimeValue(List<VariableResource> variableList, int loopThroughCount)
        {
            for (int i = 1; i <= loopThroughCount; ++i)
            {
                foreach (var variable in variableList)
                {
                    if (!string.IsNullOrWhiteSpace(variable.Value) && !string.IsNullOrWhiteSpace(DeploymentTimeValue))
                    {
                        DeploymentTimeValue = VariableRegExHelper.VariableValueWithReplacedText(DeploymentTimeValue, variable.Name, variable.Value);
                    }
                }
            }
        }

        public void LoadReferencedScriptModules(List<VariableResource> scriptList)
        {
            foreach (var scriptModule in scriptList)
            {
                if (!string.IsNullOrWhiteSpace(scriptModule.Value) && VariableRegExHelper.OctScriptTextContainsVariable(scriptModule.Value, Name))
                {
                    ScriptModuleList.Add(scriptModule);
                }
            }
        }

        public void LoadReferencedFiles(Dictionary<string, string> buildConfigDictionary)
        {
            foreach (var buildConfig in buildConfigDictionary)
            {
                if (VariableRegExHelper.VarTextContainsVariable(buildConfig.Value, Name))
                {
                    FileList.Add(buildConfig.Key);
                }
            }
        }

        public void LoadReferencedDeploymentProcesses(Dictionary<ProjectResource, DeploymentProcessResource> deploymentProcessDictionary)
        {
            foreach (var deploymentProcess in deploymentProcessDictionary)
            {
                if (deploymentProcess.Key.IncludedLibraryVariableSetIds.Contains(LibrarySetId))
                {
                    var steps = deploymentProcess.Value.Steps;
                    if (steps.Count > 0)
                    {
                        var stepActions = steps.SelectMany(x => x.Actions);
                        if (stepActions.Count() > 0)
                        {
                            var stepActionProperties = stepActions.SelectMany(x => x.Properties).Where(x => x.Key.Contains(ResourceStrings.ScripPropertyType));
                            if (stepActionProperties.Count() > 0)
                            {
                                var scriptPropertyScriptsWithVar = stepActionProperties.Where(y => VariableRegExHelper.OctScriptTextContainsVariable(y.Value.Value, Name));
                                if (scriptPropertyScriptsWithVar.Count() > 0)
                                {
                                    ProjectScriptList.Add(deploymentProcess.Key);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadReferencedStepTemplateScripts(List<ActionTemplateResource> stepTemplateList)
        {
            foreach (var stepTemplate in stepTemplateList)
            {
                var stepTemplateProperties = stepTemplate.Properties;
                if (stepTemplateProperties.Count > 0)
                {
                    var stepTemplatePropertyScripts = stepTemplateProperties.Where(x => x.Key.Contains(ResourceStrings.ScripPropertyType));
                    if (stepTemplatePropertyScripts.Count() > 0)
                    {
                        var stepTemplatePropertyScriptsWithVar = stepTemplatePropertyScripts.Where(x => VariableRegExHelper.OctScriptTextContainsVariable(x.Value.Value, Name));
                        if (stepTemplatePropertyScriptsWithVar.Count() > 0)
                        {
                            StepTemplateScriptList.Add(stepTemplate);
                        }
                    }
                }
            }
        }

        public void LoadReferencedProjectVariables(List<ProjectResource> projectList)
        {
            foreach(var project in projectList)
            {
                var projectVariables = VariableSetHelper.GetVariableSetFromProject(OctopusRepo, project);
                if(projectVariables.Variables.Count > 0)
                {
                    var projectVariablesWithVar = projectVariables.Variables.Where(x => VariableRegExHelper.VarTextContainsVariable(x.Value, Name));
                    if(projectVariablesWithVar.Count() > 0)
                    {
                        ProjectVariablesList.Add(project);
                    }
                }

            }
        }

        public void LoadRefrencedStepTemplateVariables(List<ActionTemplateResource> stepTemplateList)
        {
            foreach (var stepTemplate in stepTemplateList)
            {
                var stepTemplateVariables = stepTemplate.Parameters;
                if (stepTemplateVariables.Count > 0)
                {
                    var stepTemplateVariablesWithVar = stepTemplateVariables.Where(x => VariableRegExHelper.VarTextContainsVariable(x.DefaultValue.Value, Name));
                    if (stepTemplateVariablesWithVar.Count() > 0)
                    {
                        StepTemplateVariablesList.Add(stepTemplate);
                    }
                    else
                    {
                        var stepTemplateVariablesDisplaySettings = stepTemplateVariables.SelectMany(x => x.DisplaySettings);
                        {
                            if (stepTemplateVariablesDisplaySettings.Count() > 0)
                            {
                                var stepTemplateVariablesDisplaySettingsWithVar = stepTemplateVariablesDisplaySettings.Where(x => VariableRegExHelper.VarTextContainsVariable(x.Value, Name));
                                if (stepTemplateVariablesDisplaySettingsWithVar.Count() > 0)
                                {
                                    StepTemplateVariablesList.Add(stepTemplate);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
