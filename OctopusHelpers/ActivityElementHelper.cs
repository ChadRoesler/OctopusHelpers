using System.Collections.Generic;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class ActivityElementHelper
    {
        /// <summary>
        /// Gathers log info based on the list of the Statuses To Check
        /// </summary>
        /// <param name="activityElementToProcess"></param>
        /// <param name="tabIndex"></param>
        /// <param name="statusesToCheck"></param>
        /// <returns>Formatted Message text.</returns>
        public static string GetLogInfo(ActivityElement activityElementToProcess, int tabIndex, List<ActivityStatus> statusesToCheck = null)
        {
            var output = string.Empty;
            var tabCount = new string(' ', tabIndex * 5);
            if (statusesToCheck == null)
            {
                output += activityElementToProcess.Name + ResourceStrings.Return;
                foreach (var activityLogElement in activityElementToProcess.LogElements)
                {
                    output += (string.Format(ResourceStrings.ErrorPrinting, tabCount, activityLogElement.MessageText.Replace(ResourceStrings.Return, string.Format(ResourceStrings.ErrorPrinting, ResourceStrings.Return, tabCount)))) + ResourceStrings.Return;
                }
                foreach (var activityElement in activityElementToProcess.Children)
                {
                    output += GetLogInfo(activityElement, tabIndex + 1, statusesToCheck);
                }
            }
            else
            {
                if (statusesToCheck.Contains(activityElementToProcess.Status))
                {
                    output += activityElementToProcess.Name + ResourceStrings.Return;
                    foreach (var activityLogElement in activityElementToProcess.LogElements)
                    {
                        output += (string.Format(ResourceStrings.ErrorPrinting, tabCount, activityLogElement.MessageText.Replace(ResourceStrings.Return, string.Format(ResourceStrings.ErrorPrinting, ResourceStrings.Return, tabCount)))) + ResourceStrings.Return;
                    }
                    foreach (var activityElement in activityElementToProcess.Children)
                    {
                        output += GetLogInfo(activityElement, tabIndex + 1, statusesToCheck);
                    }
                }
            }
            return output.TrimEnd();
        }
    }
}
