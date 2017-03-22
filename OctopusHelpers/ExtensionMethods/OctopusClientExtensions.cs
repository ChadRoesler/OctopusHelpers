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
        /// Gathers the Events tied to a resource for auditing.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="resourceId">The resource id of the object for events returned.</param>
        /// <param name="eventCategory">The type of category of events returned [null returns all].</param>
        /// <returns>Enumberable of EventsResources.</returns>
        internal static IEnumerable<EventResource> GetResourceEvents(this IOctopusClient client, string resourceId, string eventCategory)
        {
            var events = new List<EventResource>();
            client.Paginate<EventResource>(string.Format(ResourceStrings.EventRegardingLink, client.RootDocument.Link(ResourceStrings.EventLink), resourceId, eventCategory), new { }, page =>
            {
                events.AddRange(page.Items);
                return true;
            });
            return events;
        }

        /// <summary>
        /// Gathers the Interruptions tied to a Task
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="resourceId">The resouce id to returns interruptiuons for.</param>
        /// <param name="pendingOnly">Returns only pending interruptions.</param>
        /// <returns>Enumberable of InterruptionResources.</returns>
        internal static IEnumerable<InterruptionResource> GetResourceInterruptions(this IOctopusClient client, string resourceId, bool pendingOnly)
        {
            var interruptions = new List<InterruptionResource>();
            client.Paginate<InterruptionResource>(string.Format(ResourceStrings.InterruptionRegardingLink, client.RootDocument.Link(ResourceStrings.InterruptionLink), resourceId, pendingOnly.ToString()), new { }, page =>
            {
                interruptions.AddRange(page.Items);
                return true;
            });
            return interruptions;
        }

        /// <summary>
        /// Gathers the Users from a Team.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="team">The team to return the user resources from.</param>
        /// <returns>Enumerable of UserResources.</returns>
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
        /// Clones one project from another.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="newProjectName">The new project's name.</param>
        /// <param name="newProjectDescription">The new project's description,</param>
        /// <param name="projectToClone">The project to clone from.</param>
        /// <param name="projectGroupIdForNewProject">The group the project will be placed into [null will copy the group from the projectToClone].</param>
        /// <param name="lifcycleId">The life cycle of the new project [null will copy the lifecycle from the projectToClone].</param>
        internal static void CloneProject(this IOctopusClient client, string newProjectName, string newProjectDescription, ProjectResource projectToClone, string projectGroupIdForNewProject, string lifcycleId)
        {
            var projectToCreate = new ProjectResource
            {
                Name = newProjectName,
                Description = newProjectDescription,
                ProjectGroupId = projectGroupIdForNewProject ?? projectToClone.ProjectGroupId,
                LifecycleId = lifcycleId ?? projectToClone.LifecycleId
            };
            client.Post(string.Format(ResourceStrings.CloneCommandApiFormat, projectToClone.Id), projectToCreate);
        }

        /// <summary>
        /// Sets the MaintenanceConfiguration Resource to the server.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="maintenanceConfigurationResource">The Resource to Set after changes have been applied.</param>
        internal static void SetMaintenanceConfigurationResource(this IOctopusClient client, MaintenanceConfigurationResource maintenanceConfigurationResource)
        {
            client.Put(ResourceStrings.MaintenanceConfigApi, maintenanceConfigurationResource);
        }

        /// <summary>
        /// Gets the MaintenanceConfiguration Resource to the server.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <returns>The Servers' MaintenanceConfiguration.</returns>
        internal static MaintenanceConfigurationResource GetMaintenanceConfigurationResource(this IOctopusClient client)
        {
            return client.Get<MaintenanceConfigurationResource>(ResourceStrings.MaintenanceConfigApi);
        }
    }
}
