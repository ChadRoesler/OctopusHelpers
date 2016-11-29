using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Used for Library Variable Sets.  These are the Script Modules and Variable sets that can be shared between projects.
    /// </summary>
    public static class LibraryVariableSetHelper
    {
        /// <summary>
        /// Returns the names of the LibraryVariableSet Script Modules
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetScriptModuleNames(OctopusRepository octRepository)
        {
            return octRepository.LibraryVariableSets.FindAll().Where(x => x.ContentType == VariableSetContentType.ScriptModule).Select(x => x.Name);
        }

        /// <summary>
        /// Returns the names of the LibraryVariableSet Variable Sets
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetVariableSetNames(OctopusRepository octRepository)
        {
            return octRepository.LibraryVariableSets.FindAll().Where(x => x.ContentType == VariableSetContentType.Variables).Select(x => x.Name);
        }

        /// <summary>
        /// Returns the names of the LibraryVariableSets
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetLibraryVariableSetNames(OctopusRepository octRepository)
        {
            return octRepository.LibraryVariableSets.FindAll().Select(x => x.Name);
        }

        /// <summary>
        /// Returns the names of the LibraryVariableSet Script Modules
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetScriptModuleIds(OctopusRepository octRepository)
        {
            return octRepository.LibraryVariableSets.FindAll().Where(x => x.ContentType == VariableSetContentType.ScriptModule).Select(x => x.Id);
        }

        /// <summary>
        /// Returns the names of the LibraryVariableSet Variable Sets
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetVariableSetIds(OctopusRepository octRepository)
        {
            return octRepository.LibraryVariableSets.FindAll().Where(x => x.ContentType == VariableSetContentType.Variables).Select(x => x.Id);
        }

        /// <summary>
        /// Returns the names of the LibraryVariableSets
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetLibraryVariableSetIds(OctopusRepository octRepository)
        {
            return octRepository.LibraryVariableSets.FindAll().Select(x => x.Id);
        }

        /// <summary>
        /// Gathers a single LibraryVariableSet based on the Id.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSetId"></param>
        /// <returns></returns>
        public static LibraryVariableSetResource GetLibraryVariableSetById(OctopusRepository octRepository, string libraryVariableSetId)
        {
            var numberOnly = new int();
            if (int.TryParse(libraryVariableSetId, out numberOnly))
            {
                return octRepository.LibraryVariableSets.Get(string.Format(ResourceStrings.LibraryVariableSetIdFormat, libraryVariableSetId));
            }
            else
            {
                return octRepository.LibraryVariableSets.Get(libraryVariableSetId);
            }
        }

        /// <summary>
        /// Gathers a single LibraryVariableSet based on the Name.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSetName"></param>
        /// <returns></returns>
        public static LibraryVariableSetResource GetLibraryVariableSetByName(OctopusRepository octRepository, string libraryVariableSetName)
        {
            return octRepository.LibraryVariableSets.FindByName(libraryVariableSetName);
        }

        /// <summary>
        /// Gathers the LibraryVariableSets that are attached to a project and returns it as a LibraryVariableSetResources List.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<LibraryVariableSetResource> GetLibararyVariableSetFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var libraryVariableSetList = new List<LibraryVariableSetResource>();
            foreach (var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                libraryVariableSetList.Add(octRepository.LibraryVariableSets.Get(libraryVariableSetId));
            }
            return libraryVariableSetList;
        }

        /// <summary>
        /// Gathers the LibraryVariableSet ScriptModules that are attached to a project and returns it as a LibraryVariableSetResources List.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<LibraryVariableSetResource> GetLibararyVariableSetScriptModulesFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var libraryVariableSetList = new List<LibraryVariableSetResource>();
            foreach (var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                var libraryVariableSet = octRepository.LibraryVariableSets.Get(libraryVariableSetId);
                if (libraryVariableSet.ContentType == VariableSetContentType.ScriptModule)
                {
                    libraryVariableSetList.Add(libraryVariableSet);
                }
            }
            return libraryVariableSetList;
        }

        /// <summary>
        /// Gathers the LibraryVariableSet Variabless that are attached to a project and returns it as a LibraryVariableSetResources List.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<LibraryVariableSetResource> GetLibararyVariableSetVariablesFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var libraryVariableSetList = new List<LibraryVariableSetResource>();
            foreach (var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                var libraryVariableSet = octRepository.LibraryVariableSets.Get(libraryVariableSetId);
                if (libraryVariableSet.ContentType == VariableSetContentType.Variables)
                {
                    libraryVariableSetList.Add(libraryVariableSet);
                }
            }
            return libraryVariableSetList;
        }

        /// <summary>
        /// Adds the LibraryVariableSet to the passed Project.
        /// </summary>
        /// <param name="Repository"></param>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSet"></param>
        public static void AddLibararyVariableSetToProject(OctopusRepository octRepository, ProjectResource project, LibraryVariableSetResource libraryVariableSet)
        {
            project.IncludedLibraryVariableSetIds.Add(libraryVariableSet.Id);
            octRepository.Projects.Modify(project);
        }

        /// <summary>
        /// Removes the LibraryVariableSet to the passed Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="libraryVariableSet"></param>
        public static void RemoveLibararyVariableSetFromProject(OctopusRepository octRepository, ProjectResource project, LibraryVariableSetResource libraryVariableSet)
        {
            project.IncludedLibraryVariableSetIds.Remove(libraryVariableSet.Id);
            octRepository.Projects.Modify(project);
        }

        /// <summary>
        /// Adds only the missing LibraryVariableSets to the passed Project.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="libraryVariableSetList"></param>
        public static void AddMissingLibararyVariableSetsToProject(OctopusRepository octRepository, ProjectResource project, List<LibraryVariableSetResource> libraryVariableSetList)
        {
            foreach (var libraryVariableToSet in libraryVariableSetList)
            {
                if (!project.IncludedLibraryVariableSetIds.Exists(x => x.Equals(libraryVariableToSet.Id)))
                {
                    project.IncludedLibraryVariableSetIds.Add(libraryVariableToSet.Id);
                }
            }
            octRepository.Projects.Modify(project);
        }

        /// <summary>
        /// Removes any LibraryVariableSets from the passed Project that are not in the passed List.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="libraryVariableSetList"></param>
        public static void RemoveExtraLibararyVariableSetsFromProject(OctopusRepository octRepository, ProjectResource project, List<LibraryVariableSetResource> libraryVariableSetList)
        {
            foreach (var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                if (!libraryVariableSetList.Select(l => l.Id).ToList().Exists(x => x.Equals(libraryVariableSetId)))
                {
                    project.IncludedLibraryVariableSetIds.Remove(libraryVariableSetId);
                }
            }
            octRepository.Projects.Modify(project);
        }

        /// <summary>
        /// Replaces the LibraryVariableSet in the passed Project with those in the passed List.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="project"></param>
        /// <param name="libraryVariableSetList"></param>
        public static void ReplaceLibararyVariableSetInProject(OctopusRepository octRepository, ProjectResource project, List<LibraryVariableSetResource> libraryVariableSetList)
        {
            project.IncludedLibraryVariableSetIds = libraryVariableSetList.Select(x => x.Id).ToList();
            octRepository.Projects.Modify(project);
        }

        /// <summary>
        /// Creates a new ScriptModule LibraryVariableSet from a passed file.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="scriptModuleName"></param>
        /// <param name="description"></param>
        /// <param name="scriptModuleText"></param>
        public static void CreateScriptModuleFromText(OctopusRepository octRepository, string scriptModuleName, string description, string scriptModuleText)
        {
            var libraryVariableSet = new LibraryVariableSetResource
            {
                ContentType = VariableSetContentType.ScriptModule,
                Description = description,
                Name = scriptModuleName
            };
            var outputLibraryVariableSet = octRepository.LibraryVariableSets.Create(libraryVariableSet);
            var variableSet = octRepository.VariableSets.Get(outputLibraryVariableSet.VariableSetId);
            variableSet.Variables = new List<VariableResource>
            { new VariableResource
                {
                    Name = string.Format(ResourceStrings.ScriptModuleNameFormat, scriptModuleName),
                    Value = scriptModuleText
                }
            };
            octRepository.VariableSets.Modify(variableSet);
        }

        /// <summary>
        /// Replaces a ScriptModule LibraryVariableSet from a passed string.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSet"></param>
        /// <param name="scriptModuleText"></param>
        public static void ReplaceScriptModuleFromText(OctopusRepository octRepository, LibraryVariableSetResource libraryVariableSet, string scriptModuleText)
        {
            var variableSet = octRepository.VariableSets.Get(libraryVariableSet.VariableSetId);
            variableSet.Variables.FirstOrDefault().Value = scriptModuleText;
            octRepository.VariableSets.Modify(variableSet);
        }

        /// <summary>
        /// Outputs a ScriptModule LibraryVariableSet to a string.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSet"></param>
        public static string OutputScriptModuleToText(OctopusRepository octRepository, LibraryVariableSetResource libraryVariableSet)
        {
            var variableSet = octRepository.VariableSets.Get(libraryVariableSet.VariableSetId);
            return variableSet.Variables.FirstOrDefault().Value;
        }

        /// <summary>
        /// Creates a new VariableSet LibraryVariableSet from a passed string.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="variableSetName"></param>
        /// <param name="description"></param>
        /// <param name="variableSetText"></param>
        public static void CreateVariableSetFromText(OctopusRepository octRepository, string variableSetName, string description, string variableSetText)
        {
            var newVariables = new List<VariableResource>();
            var variables = JsonConvert.DeserializeObject<List<VariableResource>>(variableSetText);
            var libraryVariableSet = new LibraryVariableSetResource
            {
                ContentType = VariableSetContentType.Variables,
                Description = description,
                Name = variableSetName
            };
            var outputLibraryVariableSet = octRepository.LibraryVariableSets.Create(libraryVariableSet);
            var variableSet = octRepository.VariableSets.Get(outputLibraryVariableSet.VariableSetId);
            foreach (var variableToAdd in variables)
            {
                var newVariable = new VariableResource()
                {
                    Name = variableToAdd.Name,
                    Value = variableToAdd.Value,
                    Scope = variableToAdd.Scope,
                    IsSensitive = variableToAdd.IsSensitive,
                    IsEditable = variableToAdd.IsEditable,
                    Prompt = variableToAdd.Prompt
                };
                if (newVariable.IsSensitive && newVariable.Value == null)
                {
                    newVariable.Value = string.Empty;
                }
                newVariables.Add(newVariable);
            }
            variableSet.Variables = newVariables;
            octRepository.VariableSets.Modify(variableSet);
        }

        /// <summary>
        /// Replaces a VariableSet LibraryVariableSet from a passed string.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSet"></param>
        /// <param name="variableSetText"></param>
        public static void ReplaceVariableSetFromText(OctopusRepository octRepository, LibraryVariableSetResource libraryVariableSet, string variableSetText)
        {
            var newVariables = new List<VariableResource>();
            var variables = JsonConvert.DeserializeObject<List<VariableResource>>(variableSetText);
            var variableSet = octRepository.VariableSets.Get(libraryVariableSet.VariableSetId);
            foreach (var variableToAdd in variables)
            {
                var newVariable = new VariableResource()
                {
                    Name = variableToAdd.Name,
                    Value = variableToAdd.Value,
                    Scope = variableToAdd.Scope,
                    IsSensitive = variableToAdd.IsSensitive,
                    IsEditable = variableToAdd.IsEditable,
                    Prompt = variableToAdd.Prompt
                };
                if (newVariable.IsSensitive && newVariable.Value == null)
                {
                    newVariable.Value = string.Empty;
                }
                newVariables.Add(newVariable);
            }
            variableSet.Variables = newVariables;
            octRepository.VariableSets.Modify(variableSet);
        }

        /// <summary>
        /// Outputs a VariableSet LibraryVariableSet to a string.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="libraryVariableSet"></param>
        public static string OutputVariableSetToText(OctopusRepository octRepository, LibraryVariableSetResource libraryVariableSet)
        {
            var variableSet = octRepository.VariableSets.Get(libraryVariableSet.VariableSetId);
            return JsonConvert.SerializeObject(variableSet.Variables, Formatting.Indented);
        }
    }
}