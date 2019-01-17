using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public static class StringUtilities
    {
        /// <summary>
        /// Converts a string into a numeric key.
        /// </summary>
        /// <param name="sourceString">a string of any length</param>
        /// <param name="numCharacters">number of characters to use for computing the key</param>
        /// <param name="fillerCharacter">the character to use for padding strings shorter than numCharacters</param>
        /// <returns>the key</returns>
        public static int StringToKey(string sourceString, int numCharacters, char fillerCharacter)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return 0;
            }

            var beginning = string.Concat(sourceString.Take(numCharacters));
            if (beginning.Length < numCharacters)
            {
                beginning.PadRight(numCharacters, fillerCharacter);
            }
            return beginning.Sum(x => (int)x);
        }

        /// <summary>
        /// Splits a string into two parts if the string is too long and a suitable breakpoint is 
        /// found. The breakpoint is the occurrence of splitChar nearest to index maxLength - 1.
        /// </summary>
        /// <param name="inputString">the string to split</param>
        /// <param name="maxLength">preferred maximum length</param>
        /// <param name="splitChar">the character at which the string can be divided</param>
        /// <returns>a collection with one element (the original string), if the string length is 
        /// less than or equal to maxLength. A collection with two elements, neither including the 
        /// breakpoint character, if the string was split.</returns>
        public static IEnumerable<string> SplitString(string inputString, int maxLength, char splitChar)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException();
            }

            var unchanged = new List<string>() { inputString };
            if (inputString.Length <= maxLength)
            {
                return unchanged;
            }

            var splitPosBefore = inputString.LastIndexOf(splitChar, maxLength - 1);
            var splitPosAfter = inputString.IndexOf(splitChar, maxLength - 1);

            int splitPos;
            if (splitPosBefore == -1 && splitPosAfter == -1)
            {
                splitPos = -1;
            }
            else if (splitPosBefore == -1)
            {
                splitPos = splitPosAfter;
            }
            else if (splitPosAfter == -1)
            {
                splitPos = splitPosBefore;
            }
            else
            {
                var distBefore = Math.Abs(maxLength - 1 - splitPosBefore);
                var distAfter = Math.Abs(maxLength - 1 - splitPosAfter);
                splitPos = distBefore > distAfter ? splitPosAfter : splitPosBefore;
            }

            if (splitPos == -1 || splitPos == 0 || splitPos == inputString.Length - 1)
            {
                return unchanged;
            }

            var parts = new List<string>();
            parts.Add(inputString.Substring(0, splitPos));
            parts.Add(inputString.Substring(splitPos + 1));
            return parts;
        }

        /// <summary>
        /// Changes the first letter of a string to uppercase.
        /// </summary>
        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }
    }
}
