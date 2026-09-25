using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BulkBatcher
{
    /// <summary>
    /// Minimal ".env" file reader so configuration and credentials live outside source control.
    ///
    /// Values are resolved in this order:
    ///   1. A real environment variable of the same name (handy for CI or one-off overrides).
    ///   2. The matching key in the .env file.
    ///   3. The default supplied by the caller, if any.
    ///
    /// Supported syntax: KEY=VALUE, one per line. Blank lines and lines starting with '#'
    /// are ignored, as is an optional leading "export ". Values may be wrapped in single or
    /// double quotes, which is how you keep leading/trailing spaces or a literal '#'.
    /// </summary>
    public static class Env
    {
        public const string FileName = ".env";

        private static readonly Dictionary<string, string> Values = Load();

        /// <summary>Full path of the .env file that was loaded, or null when none was found.</summary>
        public static string? LoadedFrom { get; private set; }

        private static Dictionary<string, string> Load()
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var path = Locate();
            if (path is null)
                return values;

            LoadedFrom = path;

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();

                if (line.Length == 0 || line[0] == '#')
                    continue;

                if (line.StartsWith("export ", StringComparison.Ordinal))
                    line = line["export ".Length..].TrimStart();

                var separator = line.IndexOf('=');
                if (separator <= 0)
                    continue;

                var key = line[..separator].Trim();
                var value = line[(separator + 1)..].Trim();

                if (value.Length >= 2 &&
                    ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
                {
                    value = value[1..^1];
                }
                else
                {
                    // An unquoted value may carry a trailing comment.
                    var comment = value.IndexOf('#');
                    if (comment >= 0)
                        value = value[..comment].TrimEnd();
                }

                values[key] = value;
            }

            return values;
        }

        /// <summary>
        /// Looks for the .env beside the executable and in the working directory, then walks up
        /// the directory tree so the program still finds it when run from bin/Debug/....
        /// </summary>
        private static string? Locate()
        {
            var startingPoints = new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };

            foreach (var startingPoint in startingPoints)
            {
                DirectoryInfo? directory = new DirectoryInfo(startingPoint);

                while (directory is not null)
                {
                    var candidate = Path.Combine(directory.FullName, FileName);
                    if (File.Exists(candidate))
                        return candidate;

                    directory = directory.Parent;
                }
            }

            return null;
        }

        /// <summary>Returns the configured value, or <paramref name="fallback"/> when it is not set.</summary>
        public static string GetString(string key, string fallback)
            => Find(key) ?? fallback;

        /// <summary>Returns the configured value, or throws when it is missing or blank.</summary>
        public static string GetRequiredString(string key)
        {
            var value = Find(key);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Required setting '{key}' was not found. Add it to your {FileName} file " +
                    $"(copy {FileName}.example to get started) or set it as an environment variable.");
            }

            return value;
        }

        public static int GetInt(string key, int fallback)
        {
            var value = Find(key);

            if (string.IsNullOrWhiteSpace(value))
                return fallback;

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                return parsed;

            throw new InvalidOperationException($"Setting '{key}' must be a whole number, but was \"{value}\".");
        }

        public static bool GetBool(string key, bool fallback)
        {
            var value = Find(key);

            if (string.IsNullOrWhiteSpace(value))
                return fallback;

            if (bool.TryParse(value, out var parsed))
                return parsed;

            // Also accept the shorthand people habitually type in .env files.
            switch (value.Trim().ToLowerInvariant())
            {
                case "1":
                case "yes":
                case "y":
                    return true;
                case "0":
                case "no":
                case "n":
                    return false;
            }

            throw new InvalidOperationException($"Setting '{key}' must be true or false, but was \"{value}\".");
        }

        private static string? Find(string key)
        {
            var fromEnvironment = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(fromEnvironment))
                return fromEnvironment;

            return Values.TryGetValue(key, out var value) ? value : null;
        }
    }
}
