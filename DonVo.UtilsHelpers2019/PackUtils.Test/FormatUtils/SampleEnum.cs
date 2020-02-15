using System.ComponentModel;

namespace PackUtils.Test.FormatUtils
{
    public enum SampleEnum
    {
        [Description("First value")]
        Hello = 13,

        [Description("Other value")]
        World = 37,
    }

    /// <summary>
    /// An enumeration without descriptions.
    /// </summary>
    public enum SampleEnum2
    {
        Hello = 13,
        World = 37
    }
}
