using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Project Objects.
    /// </summary>
    public static class ProjectHelper
    {
        /// <summary>
        /// Gathers a Project By Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectName">Name of project to gather.</param>
        /// <returns>ProjectResource</returns>
        public static ProjectResource GetProjectByName(OctopusRepository octRepository, string projectName)
        {
            return octRepository.Projects.FindByName(projectName);
        }

        /// <summary>
        /// Gathers all Project Names.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <returns>Enumerable of ProjectResources</returns>
        public static IEnumerable<string> GetProjectNames(OctopusRepository octRepository)
        {
            return octRepository.Projects.GetAll().Select(x => x.Name);
        }

        /// <summary>
        /// Gathers a Project by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectId">Id of project to gather.</param>
        /// <returns>ProjectResource</returns>
        public static ProjectResource GetProjectById(OctopusRepository octRepository, string projectId)
        {
            var numberOnly = new int();
            if (int.TryParse(projectId, out numberOnly))
            {
                return octRepository.Projects.Get(string.Format(ResourceStrings.ProjectIdFormat, projectId));
            }
            else
            {
                return octRepository.Projects.Get(projectId);
            }
        }

        /// <summary>
        /// Creates a new Project from another Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="ProjectToCopy">Project to copy.</param>
        /// <param name="projectName">New project Name.</param>
        /// <param name="projectDescription">New project description.</param>
        /// <returns>Newly Created ProjectResource</returns>
        public static ProjectResource CopyProjectFromProject(OctopusRepository octRepository, ProjectResource projectToCopy, string projectName, string projectDescription)
        {
            var projectToCreate = new ProjectResource
            {
                Name = projectName,
                Description = projectDescription ?? string.Empty,
                ProjectGroupId = projectToCopy.ProjectGroupId,
                LifecycleId = projectToCopy.LifecycleId
            };
            return octRepository.Projects.Create(projectToCreate);
        }

        /// <summary>
        /// Creates a new Project from another Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="ProjectToCopy">Project to copy.</param>
        /// <param name="projectName">New project Name.</param>
        /// <param name="projectDescription">New project description.</param>
        /// <param name="projectGroupToAddTo">Group to add the new project to.</param>
        /// <returns>Newly Created ProjectResource</returns>
        public static ProjectResource CopyProjectFromProject(OctopusRepository octRepository, ProjectResource projectToCopy, string projectName, string projectDescription, ProjectGroupResource projectGroupToAddTo)
        {
            var projectToCreate = new ProjectResource
            {
                Name = projectName,
                Description = projectDescription ?? string.Empty,
                ProjectGroupId = projectGroupToAddTo.Id ?? projectToCopy.ProjectGroupId,
                LifecycleId = projectToCopy.LifecycleId
            };

            return octRepository.Projects.Create(projectToCreate);
        }

        /// <summary>
        /// Creates a new Project from another Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="ProjectToCopy">Project to copy.</param>
        /// <param name="projectName">New project Name.</param>
        /// <param name="projectDescription">New project description.</param>
        /// <param name="lifecycleToAdd">Lifecycle to add the to the new Project.</param>
        /// <returns>Newly created Project Resource</returns>
        public static ProjectResource CopyProjectFromProject(OctopusRepository octRepository, ProjectResource projectToCopy, string projectName, string projectDescription, LifecycleResource lifecycleToAdd)
        {
            var projectToCreate = new ProjectResource
            {
                Name = projectName,
                Description = projectDescription ?? string.Empty,
                ProjectGroupId =  projectToCopy.ProjectGroupId,
                LifecycleId = lifecycleToAdd.Id ?? projectToCopy.LifecycleId
            };

            return octRepository.Projects.Create(projectToCreate);
        }

        /// <summary>
        /// Creates a new Project from another Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="ProjectToCopy">Project to copy.</param>
        /// <param name="projectName">New project Name.</param>
        /// <param name="projectDescription">New project description.</param>
        /// <param name="projectGroupToAddTo">Group to add the new project to.</param>
        /// <param name="lifecycleToAdd">Lifecycle to add the to the new Project.</param>
        /// <returns>Newly created Project Resource</returns>
        public static ProjectResource CopyProjectFromProject(OctopusRepository octRepository, ProjectResource projectToCopy, string projectName, string projectDescription, ProjectGroupResource projectGroupToAddTo, LifecycleResource lifecycleToAdd)
        {
            var projectToCreate = new ProjectResource
            {
                Name = projectName,
                Description = projectDescription ?? string.Empty,
                ProjectGroupId = projectGroupToAddTo.Id ?? projectToCopy.ProjectGroupId,
                LifecycleId = lifecycleToAdd.Id ?? projectToCopy.LifecycleId
            };

            return octRepository.Projects.Create(projectToCreate);
        }

        /// <summary>
        /// Deletes the specified Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="projectToDelete">Project to Delete</param>
        public static void DeleteProject(OctopusRepository octRepository, ProjectResource projectToDelete)
        {
            octRepository.Projects.Delete(projectToDelete);
        }

        /// <summary>
        /// Gets a project based on the Task Passed
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to gather the project from.</param>
        /// <returns>ProjectResource</returns>
        public static ProjectResource GetProjectFromTask(OctopusRepository octRepository, TaskResource task)
        {
            var project = new ProjectResource();
            project = octRepository.Projects.Get(DeploymentHelper.GetDeploymentFromTask(octRepository, task).ProjectId);
            return project;
        }

        /// <summary>
        /// Disables the Project Passed.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to Disable.</param>
        public static void DisableProject(OctopusRepository octRepository, ProjectResource project)
        {
            project.IsDisabled = true;
            octRepository.Projects.Modify(project);
        }
    }
}
