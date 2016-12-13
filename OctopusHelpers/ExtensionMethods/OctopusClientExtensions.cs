using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers.ExtensionMethods
{
    /// <summary>
    /// Extentions to the Client Model.
    /// </summary>
    internal static class OctopusClientExtensions
    {
        /// <summary>
        /// Gathers the Users from a Team.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        internal static IEnumerable<UserResource> GetTeamUsers(this IOctopusClient client, TeamResource team)
        {
            List<UserResource> users = new List<UserResource>();
            foreach (var userId in team.MemberUserIds)
            {
                var user = client.Get<UserResource>(string.Format(ResourceStrings.TeamUserIdFormat, userId));
                if (user != null)
                {
                    users.Add(user);
                }
            }
            return users;
        }

        /// <summary>
        /// Gathers the Queued Behind Task Resources
        /// Note: these are not full TaskResources, but seem to be TaskResource-like
        /// </summary>
        /// <param name="client"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        internal static IEnumerable<TaskResource> GetQueuedBehindTasks(this IOctopusClient client, TaskResource task)
        {
            var queuedBehind = new List<TaskResource>();
            client.Paginate<TaskResource>(task.Link(ResourceStrings.QueuedBehindLink), new
            {
            }, page =>
            {
                queuedBehind.AddRange(page.Items);
                return true;
            });
            return queuedBehind;
        }

        /// <summary>
        /// Gets all Releases for a project rather than one page's woth.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        internal static IEnumerable<ReleaseResource> GetAllReleases(this IOctopusClient client, ProjectResource project)
        {
            var releases = new List<ReleaseResource>();
            client.Paginate<ReleaseResource>(project.Link(ResourceStrings.ReleaseLink), new
            {
            }, page =>
            {
                releases.AddRange(page.Items);
                return true;
            });
            return releases;
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
