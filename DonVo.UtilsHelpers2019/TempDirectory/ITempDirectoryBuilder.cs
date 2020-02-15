namespace TempDirectory
{
    public interface ITempDirectoryBuilder
    {
        ITempDirectoryBuilder Prefix(string prefix);

        ITempDirectoryBuilder PrefixSeparator(string prefixSeparator);

        ITempDirectoryBuilder Suffix(string suffix);

        ITempDirectoryBuilder SuffixSeparator(string suffixSeparator);

        TempDirectory Create();
    }
}
