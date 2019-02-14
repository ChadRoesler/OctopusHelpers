using System.Collections.Generic;
using Octopus.Client.Model;
using OctopusHelpers.Constants;
using System.Linq;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing ActivityElements Objects.
    /// </summary>
    public static class ActivityElementHelper
    {
        /// <summary>
        /// Gathers log info based on the list of the Statuses To Check
        /// </summary>
        /// <param name="activityElementToProcess">The ActivityElement to gather info from.</param>
        /// <param name="tabIndex">Tab Index for spacing sub ActivityElements</param>
        /// <param name="statusesToCheck">Allotment to check for only specific Statuses</param>
        /// <returns>Formatted Message text from activity elements. </returns>
        public static string GetLogInfo(ActivityElement activityElementToProcess, int tabIndex, List<ActivityStatus> statusesToCheck = null)
        {
            var output = string.Empty;
            var tabCount = new string(' ', tabIndex * 5);
            if (statusesToCheck.Contains(activityElementToProcess.Status))
            {
                output += activityElementToProcess.Name + ResourceStrings.Return;
                foreach (var activityLogElement in activityElementToProcess.LogElements)
                {
                    output += (string.Format(ResourceStrings.LogPrinting, tabCount, activityLogElement.MessageText.Replace(ResourceStrings.Return, string.Format(ResourceStrings.LogPrinting, ResourceStrings.Return, tabCount)))) + ResourceStrings.Return;
                }
                foreach (var activityElement in activityElementToProcess.Children)
                {
                    output += GetLogInfo(activityElement, tabIndex + 1, statusesToCheck);
                }
            }
            return output.TrimEnd();
        }

        /// </summary>
        /// <param name="activityElementToProcess">The ActivityElement to gather info from.</param>
        /// <param name="tabIndex">Tab Index for spacing sub ActivityElements</param>
        /// <param name="statusesToCheck">Allotment to check for only specific Statuses</param>
        /// <returns>Formatted Message text from activity elements. </returns>
        public static string GetLogInfo(ActivityElement activityElementToProcess, int tabIndex)
        {
            var output = string.Empty;
            var tabCount = new string(' ', tabIndex * 5);
            output += activityElementToProcess.Name + ResourceStrings.Return;
            foreach (var activityLogElement in activityElementToProcess.LogElements)
            {
                output += (string.Format(ResourceStrings.LogPrinting, tabCount, activityLogElement.MessageText.Replace(ResourceStrings.Return, string.Format(ResourceStrings.LogPrinting, ResourceStrings.Return, tabCount)))) + ResourceStrings.Return;
            }
            foreach (var activityElement in activityElementToProcess.Children)
            {
                output += GetLogInfo(activityElement, tabIndex + 1);
            }
            return output.TrimEnd();
        }

        public static string GetStepNameById(ActivityElement activityElementToProcess, string activityElementId)
        {
            var output = string.Empty;
            var parentActivityElement = activityElementToProcess;
            var childCheck = activityElementToProcess.Children.Where(c => c.Id.Equals(activityElementId)).FirstOrDefault();
            if (childCheck == null)
            {
                foreach(var activityElement in activityElementToProcess.Children)
                {
                    
                    if(string.IsNullOrWhiteSpace(output))
                    {
                        return output;
                    }
                    else
                    {
                        output = GetStepNameById(activityElement, activityElementId);
                    }
                }
            }
            else
            {
                output = parentActivityElement.Name;
            }
            
            return output;
        }
    }
}
