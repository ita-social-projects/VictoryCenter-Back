using VictoryCenter.BLL.tests;

namespace VictoryCenter.UnitTests.TestFunctionTests;

public class TestFunctionTests
{
    private readonly TestFunction _calc = new();

    [Fact]
    public void Add_Works() => Assert.Equal(5, _calc.Add(2, 3));

    [Fact]
    public void Subtract_Works() => Assert.Equal(1, _calc.Subtract(3, 2));

    // [Fact]
    // public void Multiply_Works() => Assert.Equal(6, _calc.Multiply(2, 3));

    // [Fact]
    // public void Divide_Works() => Assert.Equal(2, _calc.Divide(6, 3));
    //
    // [Fact]
    // public void Divide_ByZero_Throws()
    //     => Assert.Throws<DivideByZeroException>(() => _calc.Divide(1, 0));
}
