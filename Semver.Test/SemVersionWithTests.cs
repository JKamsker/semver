﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Semver.Test
{
    /// <summary>
    /// Tests of the "With..." methods of <see cref="SemVersion"/>.
    /// </summary>
    public class SemVersionWithTests
    {
        public static readonly SemVersion Version = new SemVersion(1, 2, 3, "pre", "metadata");

        #region WithMajor, WithMinor, WithPatch
        [Fact]
        public void WithMajor()
        {
            var v = Version.WithMajor(42);

            Assert.Equal(new SemVersion(42, 2, 3, "pre", "metadata"), v);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void WithMajorInvalid(int majorVersion)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Version.WithMajor(majorVersion));
            Assert.StartsWith("Major version must be greater than or equal to zero.", ex.Message);
            Assert.Equal("major", ex.ParamName);
        }

        [Fact]
        public void WithMinor()
        {
            var v = Version.WithMinor(42);

            Assert.Equal(new SemVersion(1, 42, 3, "pre", "metadata"), v);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void WithMinorInvalid(int minorVersion)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Version.WithMinor(minorVersion));
            Assert.StartsWith("Minor version must be greater than or equal to zero.", ex.Message);
            Assert.Equal("minor", ex.ParamName);
        }

        [Fact]
        public void WithPatch()
        {
            var v = Version.WithPatch(42);

            Assert.Equal(new SemVersion(1, 2, 42, "pre", "metadata"), v);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void WithPatchInvalid(int patchVersion)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Version.WithPatch(patchVersion));
            Assert.StartsWith("Patch version must be greater than or equal to zero.", ex.Message);
            Assert.Equal("patch", ex.ParamName);
        }
        #endregion

        #region WithPrerelease() Overload Resolution
        /// <summary>
        /// This demonstrates that if you call <c>WithPrerelease("value")</c> it
        /// invokes <see cref="SemVersion.WithPrerelease(string,bool)"/> rather
        /// than <see cref="SemVersion.WithPrerelease(string[])"/>.
        /// </summary>
        [Fact]
        public void WithPrereleaseSingleStringCallsParsingOverload()
        {
            var v = Version.WithPrerelease("bar.baz");

            Assert.Equal(new SemVersion(1, 2, 3, "bar.baz", "metadata"), v);
        }
        #endregion

        #region WithPrerelease(string, bool)
        [Fact]
        public void WithPrereleaseParsing()
        {
            var v = Version.WithPrerelease("bar.baz.100.123abc", allowLeadingZeros: false);

            Assert.Equal(new SemVersion(1, 2, 3, "bar.baz.100.123abc", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseParsingNull()
        {
            string identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(()
                => Version.WithPrerelease(identifiers, allowLeadingZeros: false));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("prerelease", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParsingEmptyString()
        {
            var v = Version.WithPrerelease("", allowLeadingZeros: false);

            Assert.Equal(new SemVersion(1, 2, 3, "", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseParsingEmptyIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithPrerelease("bar.", allowLeadingZeros: false));
            Assert.StartsWith("Prerelease identifier cannot be empty.", ex.Message);
            Assert.Equal("prerelease", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParsingLeadingZeros()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithPrerelease("bar.0123", allowLeadingZeros: false));
            Assert.StartsWith("Leading zeros are not allowed on numeric prerelease identifiers '0123'.", ex.Message);
            Assert.Equal("prerelease", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParsingLeadingZerosAllowed()
        {
            var v = Version.WithPrerelease("bar.0123", allowLeadingZeros: true);

            Assert.Equal(new SemVersion(1, 2, 3, "bar.123", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseParsingToLarge()
        {
            var ex = Assert.Throws<OverflowException>(()
               => Version.WithPrerelease("bar.99999999999999999", allowLeadingZeros: false));
            Assert.StartsWith("Prerelease identifier '99999999999999999' was too large for Int32.", ex.Message);
        }

        [Fact]
        public void WithPrereleaseParsingInvalidCharacter()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithPrerelease("bar.abc@123", allowLeadingZeros: false));
            Assert.StartsWith("A prerelease identifier can contain only ASCII alphanumeric characters and hyphens 'abc@123'.", ex.Message);
            Assert.Equal("prerelease", ex.ParamName);
        }
        #endregion

        #region WithPrerelease(string[])
        [Fact]
        public void WithPrereleaseParams()
        {
            var v = Version.WithPrerelease("bar", "baz", "100", "123abc");

            Assert.Equal(new SemVersion(1, 2, 3, "bar.baz.100.123abc", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseParamsNull()
        {
            string[] identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithPrerelease(identifiers));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParamsEmpty()
        {
            var v = Version.WithPrerelease();

            Assert.Equal(new SemVersion(1, 2, 3, "", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseParamsEmptyIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithPrerelease("bar", ""));
            Assert.StartsWith("Prerelease identifier cannot be empty.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParamsNullIdentifier()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithPrerelease("bar", null));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParamsLeadingZeros()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithPrerelease("bar", "0123"));
            Assert.StartsWith("Leading zeros are not allowed on numeric prerelease identifiers '0123'.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseParamsToLarge()
        {
            var ex = Assert.Throws<OverflowException>(()
                => Version.WithPrerelease("bar", "99999999999999999"));
            Assert.StartsWith("Prerelease identifier '99999999999999999' was too large for Int32.", ex.Message);
        }

        [Fact]
        public void WithPrereleaseParamsInvalidCharacter()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithPrerelease("bar", "abc@123"));
            Assert.StartsWith("A prerelease identifier can contain only ASCII alphanumeric characters and hyphens 'abc@123'.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }
        #endregion

        #region WithPrerelease(IEnumerable<string>, bool)
        [Fact]
        public void WithPrereleaseEnumerable()
        {
            var v = Version.WithPrerelease(new[] { "bar", "baz", "100", "123abc" }, allowLeadingZeros: false);

            Assert.Equal(new SemVersion(1, 2, 3, "bar.baz.100.123abc", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseEnumerableNull()
        {
            IEnumerable<string> identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(()
                => Version.WithPrerelease(identifiers, allowLeadingZeros: false));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseEnumerableEmpty()
        {
            var v = Version.WithPrerelease(Enumerable.Empty<string>(), allowLeadingZeros: false);

            Assert.Equal(new SemVersion(1, 2, 3, "", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseEnumerableEmptyIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithPrerelease(new[] { "bar", "" }, allowLeadingZeros: false));
            Assert.StartsWith("Prerelease identifier cannot be empty.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseEnumerableNullIdentifier()
        {
            var ex = Assert.Throws<ArgumentNullException>(()
                => Version.WithPrerelease(new[] { "bar", null }, allowLeadingZeros: false));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseEnumerableLeadingZeros()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithPrerelease(new[] { "bar", "0123" }, allowLeadingZeros: false));
            Assert.StartsWith("Leading zeros are not allowed on numeric prerelease identifiers '0123'.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseEnumerableLeadingZerosAllowed()
        {
            var v = Version.WithPrerelease(new[] { "bar", "0123" }, allowLeadingZeros: true);

            Assert.Equal(new SemVersion(1, 2, 3, "bar.123", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseEnumerableToLarge()
        {
            var ex = Assert.Throws<OverflowException>(()
                => Version.WithPrerelease(new[] { "bar", "99999999999999999" }, allowLeadingZeros: false));
            Assert.StartsWith("Prerelease identifier '99999999999999999' was too large for Int32.", ex.Message);
        }

        [Fact]
        public void WithPrereleaseEnumerableInvalidCharacter()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithPrerelease(new[] { "bar", "abc@123" }, allowLeadingZeros: false));
            Assert.StartsWith("A prerelease identifier can contain only ASCII alphanumeric characters and hyphens 'abc@123'.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }
        #endregion

        #region WithPrerelease(IEnumerable<PrereleaseIdentifier>)
        [Fact]
        public void WithPrereleaseIdentifiers()
        {
            var v = Version.WithPrerelease(new[]
            {
                new PrereleaseIdentifier("bar"),
                new PrereleaseIdentifier("baz"),
                new PrereleaseIdentifier("100"),
                new PrereleaseIdentifier("123abc")
            });

            Assert.Equal(new SemVersion(1, 2, 3, "bar.baz.100.123abc", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseIdentifiersNull()
        {
            IEnumerable<PrereleaseIdentifier> identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithPrerelease(identifiers));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithPrereleaseIdentifiersEmpty()
        {
            var v = Version.WithPrerelease(Enumerable.Empty<PrereleaseIdentifier>());

            Assert.Equal(new SemVersion(1, 2, 3, "", "metadata"), v);
        }

        [Fact]
        public void WithPrereleaseIdentifiersDefaultIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithPrerelease(new[]
            {
                new PrereleaseIdentifier("bar"), default
            }));
            Assert.StartsWith("Prerelease identifier cannot be default/null.", ex.Message);
            Assert.Equal("prereleaseIdentifiers", ex.ParamName);
        }
        #endregion

        [Fact]
        public void WithoutPrerelease()
        {
            var v = Version.WithoutPrerelease();

            Assert.Equal(new SemVersion(1, 2, 3, "", "metadata"), v);
        }

        #region WithMetadata() Overload Resolution
        /// <summary>
        /// This demonstrates that if you call <c>WithMetadata("value")</c> it
        /// invokes <see cref="SemVersion.WithMetadata(string)"/> rather
        /// than <see cref="SemVersion.WithMetadata(string[])"/>.
        /// </summary>
        [Fact]
        public void WithMetadataSingleStringCallsParsingOverload()
        {
            var v = Version.WithMetadata("bar.baz");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.baz"), v);
        }
        #endregion

        #region WithMetadata(string)
        [Fact]
        public void WithMetadataParsing()
        {
            var v = Version.WithMetadata("bar.baz.100.123abc");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.baz.100.123abc"), v);
        }

        [Fact]
        public void WithMetadataParsingNull()
        {
            string identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(()
                => Version.WithMetadata(identifiers));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("metadata", ex.ParamName);
        }

        [Fact]
        public void WithMetadataParsingEmptyString()
        {
            var v = Version.WithMetadata("");

            Assert.Equal(new SemVersion(1, 2, 3, "pre"), v);
        }

        [Fact]
        public void WithMetadataParsingEmptyIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithMetadata("bar."));
            Assert.StartsWith("Metadata identifier cannot be empty.", ex.Message);
            Assert.Equal("metadata", ex.ParamName);
        }

        [Fact]
        public void WithMetadataParsingLeadingZeros()
        {
            var v = Version.WithMetadata("bar.0123");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.0123"), v);
        }

        [Fact]
        public void WithMetadataParsingToLarge()
        {
            var v = Version.WithMetadata("bar.99999999999999999");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.99999999999999999"), v);
        }

        [Fact]
        public void WithMetadataParsingInvalidCharacter()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithMetadata("bar.abc@123"));
            Assert.StartsWith("A metadata identifier can contain only ASCII alphanumeric characters and hyphens 'abc@123'.", ex.Message);
            Assert.Equal("metadata", ex.ParamName);
        }
        #endregion

        #region WithMetadata(string[])
        [Fact]
        public void WithMetadataParams()
        {
            var v = Version.WithMetadata("bar", "baz", "100", "123abc");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.baz.100.123abc"), v);
        }

        [Fact]
        public void WithMetadataParamsNull()
        {
            string[] identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithMetadata(identifiers));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataParamsEmpty()
        {
            var v = Version.WithMetadata();

            Assert.Equal(new SemVersion(1, 2, 3, "pre"), v);
        }

        [Fact]
        public void WithMetadataParamsEmptyIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithMetadata("bar", ""));
            Assert.StartsWith("Metadata identifier cannot be empty.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataParamsNullIdentifier()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithMetadata("bar", null));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataParamsLeadingZeros()
        {
            var v = Version.WithMetadata("bar", "0123");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.0123"), v);
        }

        [Fact]
        public void WithMetadataParamsToLarge()
        {
            var v = Version.WithMetadata("bar", "99999999999999999");

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.99999999999999999"), v);
        }

        [Fact]
        public void WithMetadataParamsInvalidCharacter()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithMetadata("bar", "abc@123"));
            Assert.StartsWith("A metadata identifier can contain only ASCII alphanumeric characters and hyphens 'abc@123'.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }
        #endregion

        #region WithMetadata(IEnumerable<string>)
        [Fact]
        public void WithMetadataEnumerable()
        {
            var v = Version.WithMetadata(new List<string> { "bar", "baz", "100", "123abc" });

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.baz.100.123abc"), v);
        }

        [Fact]
        public void WithMetadataEnumerableNull()
        {
            IEnumerable<string> identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithMetadata(identifiers));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataEnumerableEmpty()
        {
            var v = Version.WithMetadata(Enumerable.Empty<string>());

            Assert.Equal(new SemVersion(1, 2, 3, "pre"), v);
        }

        [Fact]
        public void WithMetadataEnumerableEmptyIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithMetadata(new List<string> { "bar", "" }));
            Assert.StartsWith("Metadata identifier cannot be empty.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataEnumerableNullIdentifier()
        {
            var ex = Assert.Throws<ArgumentNullException>(()
                => Version.WithMetadata(new List<string> { "bar", null }));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataEnumerableLeadingZeros()
        {
            var v = Version.WithMetadata(new List<string> { "bar", "0123" });

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.0123"), v);
        }

        [Fact]
        public void WithMetadataEnumerableToLarge()
        {
            var v = Version.WithMetadata(new List<string> { "bar", "99999999999999999" });

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.99999999999999999"), v);
        }

        [Fact]
        public void WithMetadataEnumerableInvalidCharacter()
        {
            var ex = Assert.Throws<ArgumentException>(()
                => Version.WithMetadata(new List<string> { "bar", "abc@123" }));
            Assert.StartsWith("A metadata identifier can contain only ASCII alphanumeric characters and hyphens 'abc@123'.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }
        #endregion

        #region WithMetadata(IEnumerable<MetadataIdentifier>)
        [Fact]
        public void WithMetadataIdentifiers()
        {
            var v = Version.WithMetadata(new[]
            {
                new MetadataIdentifier("bar"),
                new MetadataIdentifier("baz"),
                new MetadataIdentifier("100"),
                new MetadataIdentifier("123abc")
            });

            Assert.Equal(new SemVersion(1, 2, 3, "pre", "bar.baz.100.123abc"), v);
        }

        [Fact]
        public void WithMetadataIdentifiersNull()
        {
            IEnumerable<MetadataIdentifier> identifiers = null;
            var ex = Assert.Throws<ArgumentNullException>(() => Version.WithMetadata(identifiers));
            Assert.StartsWith("Value cannot be null.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }

        [Fact]
        public void WithMetadataIdentifiersEmpty()
        {
            var v = Version.WithMetadata(Enumerable.Empty<MetadataIdentifier>());

            Assert.Equal(new SemVersion(1, 2, 3, "pre"), v);
        }

        [Fact]
        public void WithMetadataIdentifiersDefaultIdentifier()
        {
            var ex = Assert.Throws<ArgumentException>(() => Version.WithMetadata(new[]
            {
                new MetadataIdentifier("bar"), default
            }));
            Assert.StartsWith("Metadata identifier cannot be default/null.", ex.Message);
            Assert.Equal("metadataIdentifiers", ex.ParamName);
        }
        #endregion

        [Fact]
        public void WithoutMetadata()
        {
            var v = Version.WithoutMetadata();

            Assert.Equal(new SemVersion(1, 2, 3, "pre"), v);
        }

        [Fact]
        public void WithoutPrereleaseOrMetadata()
        {
            var v = Version.WithoutPrereleaseOrMetadata();

            Assert.Equal(new SemVersion(1, 2, 3), v);
        }
    }
}