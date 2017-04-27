using System.Text.RegularExpressions;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// This class is used for determining of a set of text contains a var in one of two distict octopus ways of storing the var in the script.
    /// </summary>
    public static class VariableRegExHelper
    {
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
