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
        /// <param name="octRepository"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static UserResource GetUserFromUserName(OctopusRepository octRepository, string userName)
        {
            return octRepository.Users.FindOne(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gathers a UserResource by Id.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
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
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUserNames(OctopusRepository octRepository)
        {
            return octRepository.Users.FindAll().Select(x => x.Username);
        }

        /// <summary>
        /// Returns a list of UserNames from UserResources
        /// </summary>
        /// <param name="octRepository"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUserIds(OctopusRepository octRepository)
        {
            return octRepository.Users.FindAll().Select(x => x.Id);
        }

        /// <summary>
        /// Creates an API Key for the passed user.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="user"></param>
        /// <param name="keyNote"></param>
        /// <returns></returns>
        public static ApiKeyResource CreateApiKeyForUser(OctopusRepository octRepository, UserResource user, string keyNote)
        {
            return octRepository.Users.CreateApiKey(user, keyNote);
        }

        /// <summary>
        /// Removes the passed API key from the user it is attached to.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="apiKey"></param>
        public static void RevokeApiKey(OctopusRepository octRepository, ApiKeyResource apiKey)
        {
            octRepository.Users.RevokeApiKey(apiKey);
        }

        /// <summary>
        /// Returns all API keys from the passed user, the actuall key text is not returned in this.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="user"></param>
        public static IEnumerable<ApiKeyResource> GetUserApiKeys(OctopusRepository octRepository, UserResource user)
        {
            return octRepository.Users.GetApiKeys(user);
        }
    }
}
