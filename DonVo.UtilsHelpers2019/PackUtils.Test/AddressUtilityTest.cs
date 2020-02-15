using System;
using Xunit;

namespace PackUtils.Test
{
    public class AddressUtilityTest
    {
        [Fact]
        public void SplittedAddress_Should_Return_Correct_OneLine_Address()
        {
            var result = AddressUtility.SplitAddress("510 Gay Street", "Unit 915A", "Nashville", "TN", "U.S.A", "37219");

            var oneLineAddress = result.Number + result.Street + ", " + result.Neighborhood + ", " + result.City + ", " + result.State + " " + result.ZipCode;

            Assert.Equal("510 Gay Street, Unit 915A, Nashville, TN 37219", oneLineAddress);
        }
    }
}
