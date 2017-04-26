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
            var formattedSearch = ResourceStrings.RegExFormatPatternVariableValueBegin + varName + ResourceStrings.RegExFormatPatternVariableValueEnd;
            var markerRegex = new Regex(formattedSearch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return markerRegex.IsMatch(textToSearch);
        }

        public static bool OctScriptTextContainsVariable(string textToSearch, string varName)
        {
            var formattedSearch = ResourceStrings.RegExFormatPatterScriptModuleBegin + varName + ResourceStrings.RegExFormatPatterScriptModuleEnd;
            var markerRegex = new Regex(formattedSearch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return markerRegex.IsMatch(textToSearch);
        }
    }
}
