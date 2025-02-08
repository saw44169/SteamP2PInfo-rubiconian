using System;
using System.Text.RegularExpressions;

namespace SteamP2PInfo
{
    static class FormatUtils
    {
        public static string NamedFormat(string fmt, string[] keyNames, params object[] args)
        {
            for (int i = 0; i < keyNames.Length; i++)
            {
                fmt = Regex.Replace(fmt, $"\\{{{keyNames[i]}(([,:][^\\}}]+)?\\}})", $"{{{i}$1");
            }
            try
            {
                return string.Format(fmt, args);
            }
            catch (FormatException)
            {
                return "[FORMAT ERROR]";
            }
        }
    }
}
