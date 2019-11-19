using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Capstone_CodeComparison.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Wraps matched strings in HTML span elements styled with a background-color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keywords">Comma-separated list of strings to be highlighted</param>
        /// <param name="cssClass">The Css color to apply</param>
        /// <param name="fullMatch">false for returning all matches, true for whole word matches only</param>
        /// <returns>string</returns>
        public static string HighlightKeyWords(this string text, string keywords, string cssClass, bool fullMatch)
        {
            if (text == String.Empty || keywords == String.Empty || cssClass == String.Empty)
                return text;
            var words = keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string match in words)
            {
                text = text.Replace(match.Trim(), "<span style=\"background-color:" + cssClass + "\">"+ match.Trim() +"</span>");
            }
            return text;
        }
    }
}