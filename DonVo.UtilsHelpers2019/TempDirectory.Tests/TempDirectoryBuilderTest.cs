using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace TempDirectory.Tests
{
    public class TempDirectoryBuilderTest
    {
        [Theory]
        [MemberData(nameof(GetTempDirectoryBuilderConfigurations))]
        public void CreatesTempDirectory(TempDirectoryBuilder tempDirectoryBuilder)
        {
            using (var tempDirectory = tempDirectoryBuilder.Create())
            {
                Assert.True(Directory.Exists(tempDirectory.FullName));
            }
        }

        [Theory]
        [MemberData(nameof(GetTempDirectoryBuilderConfigurations))]
        public void DeletesTempDirectory(TempDirectoryBuilder tempDirectoryBuilder)
        {
            var tempDirectory = tempDirectoryBuilder.Create();
            Assert.True(Directory.Exists(tempDirectory.FullName));
            tempDirectory.Dispose();

            Assert.False(Directory.Exists(tempDirectory.FullName));
        }

        [Theory]
        [MemberData(nameof(GetTempDirectoryBuilderConfigurations))]
        public void FullNameContainsName(TempDirectoryBuilder tempDirectoryBuilder)
        {
            using (var tempDirectory = tempDirectoryBuilder.Create())
            {
                Assert.Contains(tempDirectory.Name, tempDirectory.FullName);
            }
        }

        [Theory]
        [MemberData(nameof(GetTempDirectoryBuilderConfigurations))]
        public void FullNameIsNotName(TempDirectoryBuilder tempDirectoryBuilder)
        {
            using (var tempDirectory = tempDirectoryBuilder.Create())
            {
                Assert.NotEqual(tempDirectory.FullName, tempDirectory.Name);
            }
        }

        [Fact]
        public void PrefixIsInName()
        {
            using (var tempDirectory = new TempDirectoryBuilder().Prefix(Prefix).Create())
            {
                Assert.StartsWith(Prefix, tempDirectory.Name);
            }
        }

        [Fact]
        public void SuffixIsInName()
        {
            using (var tempDirectory = new TempDirectoryBuilder().Suffix(Suffix).Create())
            {
                Assert.EndsWith(Suffix, tempDirectory.Name);
            }
        }

        [Fact]
        public void SeparatorsAreInName()
        {
            using (var tempDirectory = new TempDirectoryBuilder()
                .Prefix(Prefix)
                .PrefixSeparator(PrefixSeparator)
                .Suffix(Prefix)
                .SuffixSeparator(SuffixSeparator)
                .Create())
            {
                Assert.Contains(PrefixSeparator, tempDirectory.Name);
                Assert.Contains(SuffixSeparator, tempDirectory.Name);
            }
        }

        [Fact]
        public void SeparatorsAreNotInNameWithoutPrefixOrSuffix()
        {
            using (var tempDirectory = new TempDirectoryBuilder()
                .PrefixSeparator(PrefixSeparator)
                .SuffixSeparator(SuffixSeparator)
                .Create())
            {
                Assert.DoesNotContain(PrefixSeparator, tempDirectory.Name);
                Assert.DoesNotContain(SuffixSeparator, tempDirectory.Name);
            }
        }

        [UsedImplicitly]
        public static IEnumerable<object[]> GetTempDirectoryBuilderConfigurations()
        {
            yield return new object[] { new TempDirectoryBuilder() };
            yield return new object[] { new TempDirectoryBuilder().Prefix(Prefix) };
            yield return new object[] { new TempDirectoryBuilder().Suffix(Suffix) };
            yield return new object[] { new TempDirectoryBuilder().Prefix(Prefix).Suffix(Suffix) };
            yield return new object[] { new TempDirectoryBuilder().Prefix(Prefix).PrefixSeparator(PrefixSeparator) };
            yield return new object[] { new TempDirectoryBuilder().Suffix(Prefix).SuffixSeparator(SuffixSeparator) };
            yield return new object[] { new TempDirectoryBuilder().Prefix(Prefix).PrefixSeparator(PrefixSeparator).Suffix(Prefix).SuffixSeparator(SuffixSeparator) };
        }

        private const string Prefix = "prefix";
        private const string Suffix = "suffix";
        private const string PrefixSeparator = "@";
        private const string SuffixSeparator = ".";
    }
}
