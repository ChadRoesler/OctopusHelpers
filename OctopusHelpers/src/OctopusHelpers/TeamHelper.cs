﻿using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Team Objects.
    /// </summary>
    public static class TeamHelper
    {
        /// <summary>
        /// Gathers a Team by Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="teamName">Team Name to Gather</param>
        /// <returns>TeamResource</returns>
        public static TeamResource GetTeamByName(OctopusRepository octRepository, string teamName)
        {
            return octRepository.Teams.FindByName(teamName);
        }

        /// <summary>
        /// Gathers a Team by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="teamId">Team Id to gather.</param>
        /// <returns>TeamResource</returns>
        public static TeamResource GetTeamById(OctopusRepository octRepository, string teamId)
        {
            var numberOnly = new int();
            if (int.TryParse(teamId, out numberOnly))
            {
                return octRepository.Teams.Get(string.Format(ResourceStrings.TeamIdFormat, teamId));
            }
            else
            {
                return octRepository.Teams.Get(teamId);
            }
        }

        /// <summary>
        /// Gathers a List of Users in a Team by Team.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="team">Team to get users from</param>
        /// <returns>Enumerable of UserResources</returns>
        public static IEnumerable<UserResource> GetUsersByTeam(OctopusRepository octRepository, TeamResource team)
        {
            return octRepository.Client.GetTeamUsers(team);
        }

        /// <summary>
        /// Gathers a List of Users in a Team by Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="teamName">Team name to get users from.</param>
        /// <returns>Enumerable of UserResources</returns>
        public static IEnumerable<UserResource> GetUsersByTeamName(OctopusRepository octRepository, string teamName)
        {
            var team = GetTeamByName(octRepository, teamName);
            return octRepository.Client.GetTeamUsers(team);
        }

        /// <summary>
        /// Gathers a List of Users in a Team by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="teamId">Team id to get users from.</param>
        /// <returns>Enumerable of UserResources</returns>
        public static IEnumerable<UserResource> GetUsersByTeamId(OctopusRepository octRepository, string teamId)
        {
            var team = GetTeamById(octRepository, teamId);
            return octRepository.Client.GetTeamUsers(team);
        }
    }
}
