using System;
using System.Collections.Generic;
using System.Linq;

namespace risk_of_sharing
{
    public static class StringHelper
    {
        public static string RemoveWhitespace(string input)
        {
            return new string(input.Where(character => !Char.IsWhiteSpace(character)).ToArray());
        }

        public static List<string> ParseCommaSeparatedList(string input)
        {
            return RemoveWhitespace(input).Split(",").ToList();
        }
    }
}