using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.UnitTests.HelperTests;

public class ValidationHelpersTests
{
    [Fact]
    public void HaveMaximumDigits_Long_LessThanLimit_ReturnsTrue()
    {
        var validator = ValidationHelpers.HaveMaximumDigits(5);

        var result = validator(123);

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigits_Long_EqualToLimit_ReturnsTrue()
    {
        var validator = ValidationHelpers.HaveMaximumDigits(3);

        var result = validator(123);

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigits_Long_GreaterThanLimit_ReturnsFalse()
    {
        var validator = ValidationHelpers.HaveMaximumDigits(2);

        var result = validator(123);

        Assert.False(result);
    }

    [Fact]
    public void HaveMaximumDigits_Long_Zero_ReturnsTrue()
    {
        var validator = ValidationHelpers.HaveMaximumDigits(1);

        var result = validator(0);

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigits_Long_NegativeNumber_CountsMinusSign()
    {
        var validator = ValidationHelpers.HaveMaximumDigits(3);

        var result = validator(-12); // "-12" length = 3

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigitsInt_LessThanLimit_ReturnsTrue()
    {
        var validator = ValidationHelpers.HaveMaximumDigitsInt(4);

        var result = validator(99);

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigitsInt_EqualToLimit_ReturnsTrue()
    {
        var validator = ValidationHelpers.HaveMaximumDigitsInt(2);

        var result = validator(99);

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigitsInt_GreaterThanLimit_ReturnsFalse()
    {
        var validator = ValidationHelpers.HaveMaximumDigitsInt(2);

        var result = validator(123);

        Assert.False(result);
    }

    [Fact]
    public void HaveMaximumDigitsInt_Zero_ReturnsTrue()
    {
        var validator = ValidationHelpers.HaveMaximumDigitsInt(1);

        var result = validator(0);

        Assert.True(result);
    }

    [Fact]
    public void HaveMaximumDigitsInt_NegativeNumber_CountsMinusSign()
    {
        var validator = ValidationHelpers.HaveMaximumDigitsInt(3);

        var result = validator(-12); // "-12" length = 3

        Assert.True(result);
    }
}
