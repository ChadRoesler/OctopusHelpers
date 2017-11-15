using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Forms;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    public static class ChannelHelper
    {
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

        public static IEnumerable<ChannelResource> GetChannelsFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            return octRepository.Client.GetProjectChannels(project);
        }
    }
}
