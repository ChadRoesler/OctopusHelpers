using System.Text.RegularExpressions;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// This class is used for determining of a set of text contains a var in one of two distinct octopus ways of storing the var in the script.
    /// </summary>
    /// 
    /// <note>
    /// Not using Octostache since we want counts of things.  We are only doing basic var replacement not the advanced if or other things.
    /// Ill probably end up documenting this later as its use in conjunction with the VariableInfoResource
    /// </note>
    public static class VariableRegExHelper
    {
        /// <summary>
        /// Look some other time kid
        /// </summary>
        /// <param name="textToSearch">you dont wanna</param>
        /// <param name="varName">live this life</param>
        /// <returns></returns>
        public static bool VarTextContainsVariable(string textToSearch, string varName)
        {
            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                var formattedSearch = ResourceStrings.RegExFormatPatternVariableValueBegin + varName + ResourceStrings.RegExFormatPatternVariableValueEnd;
                var markerRegex = new Regex(formattedSearch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                return markerRegex.IsMatch(textToSearch);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// going out to get milk
        /// </summary>
        /// <param name="textToSearch">be back</param>
        /// <param name="varName">in 10 years</param>
        /// <returns>never</returns>
        public static bool OctScriptTextContainsVariable(string textToSearch, string varName)
        {
            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                var formattedSearch = ResourceStrings.RegExFormatPatterScriptModuleBegin + varName + ResourceStrings.RegExFormatPatterScriptModuleEnd;
                var markerRegex = new Regex(formattedSearch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                return markerRegex.IsMatch(textToSearch);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// nope
        /// </summary>
        /// <param name="textToReplace">nuh-uh</param>
        /// <param name="varName">no way</param>
        /// <param name="varValue">jose</param>
        /// <returns>get out</returns>
        public static string VariableValueWithReplacedText(string textToReplace, string varName, string varValue)
        {
            if (!string.IsNullOrWhiteSpace(textToReplace))
            {
                var formattedSearch = ResourceStrings.RegExFormatPatternVariableValueBegin + varName + ResourceStrings.RegExFormatPatternVariableValueEnd;
                var markerRegex = new Regex(formattedSearch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                return markerRegex.Replace(textToReplace, varValue);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
