using System;

namespace Semver.Ranges.Comparers.Npm
{
    public static class SemVersionExtensions
    {
        /// <summary>
        /// <para>
        /// Checks if this version satisfies the specified range.
        /// Uses the same range syntax as npm.
        /// </para>
        /// <para>
        /// Note: It's more optimal to use the static parse methods on <see cref="NpmRange"/>
        /// if you're gonna be testing multiple versions against the same range
        /// to avoid having to parse the range multiple times.
        /// </para>
        /// </summary>
        /// <param name="version"></param>
        /// <param name="range">The range to compare with. Invalid syntax will always return false.</param>
        /// <param name="options">Optional range parsing options</param>
        public static bool SatisfiesNpm(this SemVersion version, string range, NpmParseOptions options = default)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (!NpmRange.TryParse(range, options, out var parsedRange))
                return false;

            return parsedRange.Includes(version);
        }

        /// <summary>
        /// Checks if this version satisfies the specified range.
        /// Uses the same syntax as npm.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="range">The range to compare with. Can be created from <see cref="NpmRange.Parse(string)"/> or <see cref="NpmRange.TryParse(string,out Semver.Ranges.Comparers.Npm.NpmRange)"/>.</param>
        /// <returns>True if the version satisfies the range.</returns>
        /// <exception cref="ArgumentNullException">Thrown if version or range is null.</exception>
        public static bool SatisfiesNpm(this SemVersion version, NpmRange range)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (range == null) throw new ArgumentNullException(nameof(range));

            return range.Includes(version);
        }
    }
}