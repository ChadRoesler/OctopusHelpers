using System;
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
        /// <param name="task">Task to Get interruption of.</param>
        /// <returns>InterruptionResource</returns>
        public static InterruptionResource GetPendingInterruption(OctopusRepository octRepository, TaskResource task)
        {
            return octRepository.Client.GetResourceInterruptions(task.Id, true).FirstOrDefault();
        }

        /// <summary>
        /// Gathers the tasks last interruption.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="task">Task to Get interruption of.</param>
        /// <returns>InterruptionResource</returns>
        public static InterruptionResource GetLastInterruption(OctopusRepository octRepository, TaskResource task)
        {
            return octRepository.Client.GetResourceInterruptions(task.Id, false).FirstOrDefault();
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

        /// <summary>
        /// Gathers the Step name of the interruption from the CorrelationId
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="interruption"></param>
        /// <returns></returns>
        public static string GetInterruptedStepName(OctopusRepository octRepository, InterruptionResource interruption)
        {
            var task = TaskHelper.GetTaskFromId(octRepository, interruption.TaskId);
            var activityElements = TaskHelper.GetActivityElementList(octRepository, task);
            var stepName = string.Empty;
            foreach (var activityElement in activityElements)
            {
                if (string.IsNullOrEmpty(stepName))
                {
                    stepName = ActivityElementHelper.GetStepNameById(activityElement, interruption.CorrelationId);
                }
            }
            return stepName;
        }

        /// <summary>
        /// Gets the manual Intervention Step Instructions
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="interruption"></param>
        /// <returns></returns>
        public static string GetInterventionDirections(OctopusRepository octRepository, InterruptionResource interruption)
        {
            var instructions = string.Empty;
            var interruptionformElement = interruption.Form.Elements.Where(e => e.Name.Equals(ResourceStrings.InterventionInstructions)).FirstOrDefault();
            if(interruptionformElement != null && interruptionformElement.Control.GetType() == typeof(Octopus.Client.Model.Forms.Paragraph))
            {
                instructions = ((Octopus.Client.Model.Forms.Paragraph)interruptionformElement.Control).Text;
            }
            return instructions;
        }
    }
}
