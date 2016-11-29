using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.Models;

namespace OctopusHelpers.ExtensionMethods
{
    internal static class OctopusClientExtensions
    {
        /// <summary>
        /// Correctly Returns all Projects in a ProjectGroup.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="projectGroup"></param>
        /// <returns></returns>
        internal static IEnumerable<ProjectResource> GetProjectGroupProjects(this IOctopusClient client, ProjectGroupResource projectGroup)
        {
            List<ProjectResource> projects = new List<ProjectResource>();
            client.Paginate<ProjectResource>(projectGroup.Link(ResourceStrings.ProjectsLink), new
            {
            }, page =>
            {
                projects.AddRange(page.Items);
                return true;
            });
            return projects;
        }

        /// <summary>
        /// Gathers the 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        internal static IEnumerable<QueuedBehindTaskResource> GetQueuedBehindResources(this IOctopusClient client, TaskResource task)
        {
            var queuedBehind = client.Get<IEnumerable<QueuedBehindTaskResource>>(task.Link(ResourceStrings.QueuedBehindLink));
            return queuedBehind;
        }

        /// <summary>
        /// Clones one project from another.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="newProjectName"></param>
        /// <param name="newProjectDescription"></param>
        /// <param name="projectToClone"></param>
        /// <param name="projectGroupIdForNewProject"></param>
        internal static void CloneProject(this IOctopusClient client, string newProjectName, string newProjectDescription, ProjectResource projectToClone, string projectGroupIdForNewProject)
        {
            var projectToCreate = new ProjectResource
            {
                Name = newProjectName,
                Description = newProjectDescription,
                ProjectGroupId = projectGroupIdForNewProject
            };
            client.Post(string.Format(ResourceStrings.CloneCommandApiFormat, projectToClone.Id), projectToCreate);
        }

        /// <summary>
        /// Sets the MaintenanceConfiguration Resource to the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="maintenanceConfigurationResource"></param>
        internal static void SetMaintenanceConfigurationResource(this IOctopusClient client, MaintenanceConfigurationResource maintenanceConfigurationResource)
        {
            client.Put(ResourceStrings.MaintenanceConfigApi, maintenanceConfigurationResource);
        }

        /// <summary>
        /// Gets the MaintenanceConfiguration Resource to the server.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        internal static MaintenanceConfigurationResource GetMaintenanceConfigurationResource(this IOctopusClient client)
        {
            return client.Get<MaintenanceConfigurationResource>(ResourceStrings.MaintenanceConfigApi);
        }
    }
}
