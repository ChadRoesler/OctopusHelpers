using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Octopus.Client;
using Octopus.Client.Model;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Variable Objects.
    /// </summary>
    public static class VariableSetHelper
    {
        /// <summary>
        /// Gathers the VariableSet From a Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <returns>VariableSetResource</returns>
        public static VariableSetResource GetVariableSetFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            return octRepository.VariableSets.Get(project.VariableSetId);
        }

        /// <summary>
        /// Gathers the VariableSet From a LibraryVariableSet.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="libraryVariableSet">LibraryVariableSet to gather from.</param>
        /// <returns>VariableSetResource</returns>
        public static VariableSetResource GetVariableSetFromLibraryVariableSet(OctopusRepository octRepository, LibraryVariableSetResource libraryVariableSet)
        {
            return octRepository.VariableSets.Get(libraryVariableSet.VariableSetId);
        }

        /// <summary>
        /// Adds a Variable to a Project's VariableSet.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to add to.</param>
        /// <param name="variable"></param>
        public static void AddVariableToProjectVariableSet(OctopusRepository octRepository, ProjectResource project, VariableResource variable)
        {
            var variableResource = new VariableResource();
            variableResource.IsEditable = variable.IsEditable;
            variableResource.IsSensitive = variable.IsSensitive;
            variableResource.Name = variable.Name;
            variableResource.Prompt = variable.Prompt;
            variableResource.Scope = variable.Scope;
            var currentProjectVariables = GetVariableSetFromProject(octRepository, project);
            currentProjectVariables.Variables.Add(variableResource);
            octRepository.VariableSets.Modify(currentProjectVariables);
        }

        /// <summary>
        /// Removes a Variable to a Project's VariableSet.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to remove from.</param>
        /// <param name="variable"></param>
        public static void RemoveVariableFromProjectVariableSet(OctopusRepository octRepository, ProjectResource project, VariableResource variable)
        {
            var currentProjectVariables = GetVariableSetFromProject(octRepository, project);
            var variableToRemove = currentProjectVariables.Variables.Where(x => x.Id == variable.Id).FirstOrDefault();
            currentProjectVariables.Variables.Remove(variableToRemove);
            octRepository.VariableSets.Modify(currentProjectVariables);
        }

        /// <summary>
        /// Replace's a Project's VariableSet.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <param name="variableSet">VariableSet that will be entered.</param>
        public static void ReplaceVariablesInProjectVariableSet(OctopusRepository octRepository, ProjectResource project, VariableSetResource variableSet)
        {
            var newVariableList = new List<VariableResource>();
            foreach (var variable in variableSet.Variables)
            {
                var variableResource = new VariableResource();
                variableResource.IsEditable = variable.IsEditable;
                variableResource.IsSensitive = variable.IsSensitive;
                variableResource.Name = variable.Name;
                variableResource.Prompt = variable.Prompt;
                variableResource.Scope = variable.Scope;
                newVariableList.Add(variableResource);
            }
            var currentProjectVariables = GetVariableSetFromProject(octRepository, project);
            currentProjectVariables.Variables = newVariableList;
            octRepository.VariableSets.Modify(currentProjectVariables);
        }

        /// <summary>
        /// Replaces a VariableSet Project from a passed file.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to replace variables in.</param>
        /// <param name="variableSetText">Variable set as JSON text.</param>
        public static void ReplaceProjectVariableSetFromFile(OctopusRepository octRepository, ProjectResource project, string variableSetText)
        {
            var variables = JsonConvert.DeserializeObject<List<VariableResource>>(variableSetText);
            var variableSet = GetVariableSetFromProject(octRepository, project);
            variableSet.Variables = variables;
            octRepository.VariableSets.Modify(variableSet);
        }

        /// <summary>
        /// Outputs a VariableSet Project from a passed project to a JSON string.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gather from.</param>
        /// <returns>String of Variables from Project</returns>
        public static string OutputProjectVariableSetToString(OctopusRepository octRepository, ProjectResource project)
        {
            var variableSet = GetVariableSetFromProject(octRepository, project);
            return  JsonConvert.SerializeObject(variableSet.Variables, Formatting.Indented);
        }
    }
}
