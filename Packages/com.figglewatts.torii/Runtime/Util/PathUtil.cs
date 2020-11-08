using System;
using System.IO;

namespace Torii.Util
{
    /// <summary>
    /// Various utility methods for paths.
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// Combines two elements into a path. Converts back slashes in path to forward slashes.
        /// </summary>
        /// <param name="a">Element A</param>
        /// <param name="b">Element B</param>
        /// <returns>The combined path</returns>
        public static string Combine(string a, string b)
        {
            if (b.StartsWith("\\") || b.StartsWith("/"))
            {
                b = b.Substring(1);
            }

            return Path.Combine(a, b).Replace('\\', '/');
        }

        /// <summary>
        /// Combines any number of path parameters. Converts back slashes in path to forward slashes.
        /// </summary>
        /// <param name="componentStrings">Any number of path elements to combine</param>
        /// <returns>The combined path.</returns>
        public static string Combine(params string[] componentStrings)
        {
            string path = componentStrings[0];
            for (int i = 1; i < componentStrings.Length; i++)
            {
                path = Combine(path, componentStrings[i]);
            }

            return path.Replace('\\', '/');
        }

        // from: https://stackoverflow.com/a/31941159/13166789
        /// <summary>
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\')
                                                         .WithEnding("\\"));

            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\')
                                                                       .WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string WithEnding(this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }
    }
}
