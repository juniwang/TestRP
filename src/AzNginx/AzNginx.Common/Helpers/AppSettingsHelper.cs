using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AzNginx.Common.Helpers
{
    public static class AppSettingsHelper
    {
        private static readonly char[] _arraySplitChars = new[] { ',', ';' };

        public static string[] ReadStringArray(string key)
        {
            string rawValue = ConfigurationManager.AppSettings[key];
            return ParseStringArray(rawValue);
        }

        public static string[] ParseStringArray(string rawValue)
        {
            if (rawValue == null)
            {
                return new string[0];
            }

            return rawValue
                    .Split(_arraySplitChars)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToArray();
        }

        public static IReadOnlyDictionary<string, string> ReadDictionary(string key)
        {
            var rawValue = ConfigurationManager.AppSettings[key];
            return ParseDictionary(rawValue);
        }

        public static IReadOnlyDictionary<string, string> ParseDictionary(string rawValue)
        {
            var strings = ParseStringArray(rawValue);
            var dictionary = new Dictionary<string, string>();

            foreach (var pair in strings)
            {
                var tokens = pair.Split(':')
                    .Select(s => s.Trim())
                    .ToArray();

                if (tokens.Length != 2)
                {
                    throw new InvalidOperationException("the dictionary item should have 2 parts (key:value), but instead has " + tokens.Length + " part(s)");
                }

                dictionary.Add(tokens[0], tokens[1]);
            }

            return dictionary;
        }
    }
}