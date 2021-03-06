﻿using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Gathering Event Info as this seems to be stripped from resources
    /// </summary>
    public static class EventHelper
    {
        /// <summary>
        /// Gathers events for a given resource Id
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="resourceId"></param>
        /// <param name="eventCategory"></param>
        /// <returns>Enumerable of EventResources.</returns>
        public static IEnumerable<EventResource> GetResourceEvents(OctopusRepository octRepository, string resourceId, string eventCategory = "")
        {
            return octRepository.Client.GetResourceEvents(resourceId, eventCategory);
        }
    }
}
