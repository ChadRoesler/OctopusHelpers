﻿using System.Collections.Generic;
using System.Linq;
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
        /// <param name="client"></param>
        /// <param name="resourceId"></param>
        /// <param name="eventCategory"></param>
        /// <returns></returns>
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
        /// <param name="client"></param>
        /// <param name="taskId"></param>
        /// <param name="pendingOnly"></param>
        /// <returns></returns>
        internal static IEnumerable<InterruptionResource> GetResourceInterruptions(this IOctopusClient client, string resourceId, bool pendingOnly)
        {
            var interruptions = new List<InterruptionResource>();
            client.Paginate<InterruptionResource>(string.Format(ResourceStrings.InterruptionRegardingLink, client.RootDocument.Link(ResourceStrings.EventLink), resourceId, pendingOnly.ToString()), new { }, page =>
            {
                interruptions.AddRange(page.Items);
                return true;
            });
            return interruptions;
        }

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
        /// Clones one project from another.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="newProjectName"></param>
        /// <param name="newProjectDescription"></param>
        /// <param name="projectToClone"></param>
        /// <param name="projectGroupIdForNewProject"></param>
        internal static void CloneProject(this IOctopusClient client, string newProjectName, string newProjectDescription, ProjectResource projectToClone, string projectGroupIdForNewProject = null, string lifcycleId = null)
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
