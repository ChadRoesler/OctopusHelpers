using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using OctopusHelpers.Enums;

namespace OctopusHelpers
{
    public static class InterruptionHelper
    {
        /// <summary>
        /// Gathers the tasks current pending interruption.
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static InterruptionResource GetPendingInterruption(OctopusRepository octRepository, TaskResource task)
        {
            return octRepository.Interruptions.List(0, true, task.Id).Items.LastOrDefault();
        }

        /// <summary>
        /// Responds to the interruption.
        /// </summary>
        /// <param name="octRepository"></param>
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
