﻿using System;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.Enums;
using OctopusHelpers.ExtensionMethods;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Interruption Objects.
    /// </summary>
    public static class InterruptionHelper
    {
        /// <summary>
        /// Gathers the tasks current pending interruption.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task"></param>
        /// <returns>InterruptionResource</returns>
        public static InterruptionResource GetPendingInterruption(OctopusRepository octRepository, TaskResource task)
        {
            return octRepository.Client.GetResourceInterruptions(task.Id, true).FirstOrDefault();
        }

        /// <summary>
        /// Gathers the count of the retried interruptions for a task.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task"></param>
        /// <returns>Number of Retrys</returns>
        public static int GetInterruptionRetryCount(OctopusRepository octRepository, TaskResource task)
        {
            return octRepository.Client.GetResourceInterruptions(task.Id, false).Where(x => x.Form.Values[ResourceStrings.InterruptionGuidanceKey].Equals(ResourceStrings.InterruptionRetryValue)).Count();
        }

        /// <summary>
        /// Responds to the interruption.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="interruption"></param>
        /// <param name="response"></param>
        /// <param name="note"></param>
        public static void InterruptionReponse(OctopusRepository octRepository, InterruptionResource interruption, InterruptionResponse response, string note)
        {
            var stringResponse = Enum.GetName(typeof(InterruptionResponse), response);
            interruption.Form.Values[ResourceStrings.InterruptionGuidanceKey] = stringResponse;
            interruption.Form.Values[ResourceStrings.InterruptionNoteKey] = note;
            octRepository.Interruptions.TakeResponsibility(interruption);
            octRepository.Interruptions.Submit(interruption);
        }
    }
}