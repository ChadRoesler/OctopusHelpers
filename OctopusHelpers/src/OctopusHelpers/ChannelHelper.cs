using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helper for managing Channel Objects
    /// </summary>
    public static class ChannelHelper
    {
        /// <summary>
        /// Gather the channel from the Id passed
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="channelId">Channel id to gather.</param>
        /// <returns>ChannelResource</returns>
        public static ChannelResource GetChannelFromChannelId(OctopusRepository octRepository, string channelId)
        {
            var numberOnly = new int();
            if (int.TryParse(channelId, out numberOnly))
            {
                return octRepository.Channels.Get(string.Format(ResourceStrings.ChannelIdFormat, channelId));
            }
            else
            {
                return octRepository.Channels.Get(channelId);
            }
        }

        /// <summary>
        /// Gathers the channels from the passed project
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project">Project to gathers channels from.</param>
        /// <returns>Enumerable of ChannelResources</returns>
        public static IEnumerable<ChannelResource> GetChannelsFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            return octRepository.Client.GetProjectChannels(project);
        }
    }
}
