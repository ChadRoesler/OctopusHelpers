using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing User Objects.
    /// </summary>
    public static class UserHelper
    {
        /// <summary>
        /// Gathers a UserResource by Name.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="userName">UserName to gather.</param>
        /// <returns>UserResource</returns>
        public static UserResource GetUserFromUserName(OctopusRepository octRepository, string userName)
        {
            return octRepository.Users.FindOne(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gathers a UserResource by Id.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="userId">UserID to gather.</param>
        /// <returns>UserResource</returns>
        public static UserResource GetUserFromUserId(OctopusRepository octRepository, string userId)
        {

            var numberOnly = new int();
            if (int.TryParse(userId, out numberOnly))
            {
                return octRepository.Users.Get(string.Format(ResourceStrings.UserIdFormat, userId));
            }
            else
            {
                return octRepository.Users.Get(userId);
            }
        }

        /// <summary>
        /// Returns a list of UserNames from UserResources
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <returns>Enumerable of UserNames as Strings</returns>
        public static IEnumerable<string> GetUserNames(OctopusRepository octRepository)
        {
            return octRepository.Users.FindAll().Select(x => x.Username);
        }

        /// <summary>
        /// Returns a list of UserNames from UserResources
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <returns>Enumerable of UserIs as Strings</returns>
        public static IEnumerable<string> GetUserIds(OctopusRepository octRepository)
        {
            return octRepository.Users.FindAll().Select(x => x.Id);
        }

        /// <summary>
        /// Creates an API Key for the passed user.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="user">User To Create Key For.</param>
        /// <param name="keyNote">Note regarding the key.</param>
        /// <returns>ApiKeyResource</returns>
        public static ApiKeyResource CreateApiKeyForUser(OctopusRepository octRepository, UserResource user, string keyNote)
        {
            return octRepository.Users.CreateApiKey(user, keyNote);
        }

        /// <summary>
        /// Removes the passed API key from the user it is attached to.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="apiKey">ApiKey to Revoke.</param>
        public static void RevokeApiKey(OctopusRepository octRepository, ApiKeyResource apiKey)
        {
            octRepository.Users.RevokeApiKey(apiKey);
        }

        /// <summary>
        /// Returns all API keys from the passed user, the actuall key text is not returned in this.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="user">User to gather from.</param>
        /// <returns>Enumerable of ApiKeyResources</returns>
        public static IEnumerable<ApiKeyResource> GetUserApiKeys(OctopusRepository octRepository, UserResource user)
        {
            return octRepository.Users.GetApiKeys(user);
        }

        /// <summary>
        /// Returns the Teams a user is associated to.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="user">User to gather from.</param>
        /// <returns>Enumerable of TeamResoureces</returns>
        public static IEnumerable<TeamResource> GetUserTeams(OctopusRepository octRepository, UserResource user)
        {
            var userTeams = new List<TeamResource>();
            var teamList = octRepository.Teams.FindAll().Where(t => t.MemberUserIds.ToList().Exists(u => u.Equals(user.Id)));
            return teamList;
        }
    }
}
