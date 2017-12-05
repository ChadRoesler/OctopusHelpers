using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers.ExtensionMethods
{
    /// <summary>
    /// Extensions to the Client Model.
    /// </summary>
    internal static class OctopusClientExtensions
    {
        /// <summary>
        /// Gathers the Events tied to a resource for auditing.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="resourceId">The resource id of the object for events returned.</param>
        /// <param name="eventCategory">The type of category of events returned [null returns all].</param>
        /// <returns>Enumerable of EventsResources.</returns>
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
        /// Gathers the List of 
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="projectIdList">The Project Ids to gather Deployments from.</param>
        /// <param name="environmentIdList">The Environment Ids to gather Deployments from.</param>
        /// <returns>Enumerable of Deployment Resources.</returns>
        internal static IEnumerable<DeploymentResource> GetProjectEnvironmentDeployments(this IOctopusClient client, string[] projectIdList, string[] environmentIdList)
        {
            var deployments = new List<DeploymentResource>();
            var projectIdStringList = string.Join(",", projectIdList);
            var environmentIdStringList = string.Join(",", environmentIdList);
            client.Paginate<DeploymentResource>(string.Format(ResourceStrings.DeploymentsLink, client.RootDocument.Link(ResourceStrings.DeploymentLink), projectIdStringList, environmentIdStringList), new { }, page =>
            {
                deployments.AddRange(page.Items);
                return true;
            });
            return deployments;
        }

        /// <summary>
        /// Gathers the Interruptions tied to a Task
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="resourceId">The resource id to returns interruptions for.</param>
        /// <param name="pendingOnly">Returns only pending interruptions.</param>
        /// <returns>Enumerable of InterruptionResources.</returns>
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

        /// <summary>
        /// Gets the Usage resource of the passed ActionTemplate.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="actionTemplate">The action template to get more info about.</param>
        /// <returns>The List of action template usages.</returns>
        internal static IEnumerable<ActionTemplateUsageResource> GetActionTemplateUsage(this IOctopusClient client, ActionTemplateResource actionTemplate)
        {
            return client.Get<IEnumerable<ActionTemplateUsageResource>>(actionTemplate.Link(ResourceStrings.UsageLink));
        }

        /// <summary>
        /// Gets the Channels of the passed Project
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="project">The Project to get Channels of</param>
        /// <returns></returns>
        internal static IEnumerable<ChannelResource> GetProjectChannels(this IOctopusClient client, ProjectResource project)
        {
            List<ChannelResource> channels = new List<ChannelResource>();
            client.Paginate<ChannelResource>(project.Link(ResourceStrings.ChannelLink), new { }, page =>
            {
                channels.AddRange(page.Items);
                return true;
            });
            return channels;

        }

        /// <summary>
        /// Gathers the releases Tied to a channel.
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="channel">The channel to gather</param>
        /// <returns>Enumerable of Release Resources.</returns>
        internal static IEnumerable<ReleaseResource> GetChannelReleases(this IOctopusClient client, ChannelResource channel)
        {
            List<ReleaseResource> releases = new List<ReleaseResource>();
            client.Paginate<ReleaseResource>(channel.Link(ResourceStrings.ReleaseLink), new { }, page =>
            {
                releases.AddRange(page.Items);
                return true;
            });
            return releases;
        }

        /// <summary>
        /// Gathers deployments based on release
        /// </summary>
        /// <param name="client">The Repository this is tacked on to.</param>
        /// <param name="release">The release to get deployments of.</param>
        /// <returns>Enumerable of Deployment Resources.</returns>
        internal static IEnumerable<DeploymentResource> GetReleaseDeployments(this IOctopusClient client, ReleaseResource release)
        {
            List<DeploymentResource> deployments = new List<DeploymentResource>();
            client.Paginate<DeploymentResource>(release.Link(ResourceStrings.DeploymentLink), new { }, page =>
            {
                deployments.AddRange(page.Items);
                return true;
            });
            return deployments;
        }
    }
}
