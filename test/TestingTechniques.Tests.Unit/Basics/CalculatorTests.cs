using CalculatorLibrary;
using Xunit.Abstractions;

namespace TestingTechniques.Tests.Unit.Basics;

//Basic Theory tests with InlineData

public class CalculatorTests
{
    private readonly Calculator _sut = new();

    //In order to see some output in a console, we need to inject "ITestOutputHelper" 
    private readonly ITestOutputHelper _outputHelper;

    public CalculatorTests(ITestOutputHelper outputHelper)
    {
        //xUnit will handler dependency injection
        _outputHelper = outputHelper;
        _outputHelper.WriteLine("Hello from the ctor");
    }

    //This will result in one test per one InlineData attribute
    [Theory] //We can add the skip here to skip every InlineData
    [InlineData(5, 5, 10)]
    [InlineData(-5, 5, 0)]
    [InlineData(-15, -5, -20)]
    public void Add_ShouldAddTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        _outputHelper.WriteLine("Hello from the Add test");

        // Act
        var actual = _sut.Add(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 5, 0)]
    [InlineData(15, 5, 10)]
    [InlineData(-5, -5, 0)]
    [InlineData(-15, -5, -10)]
    [InlineData(5, 10, -5)]
    public void Subtract_ShouldSubtractTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        _outputHelper.WriteLine("Hello from the Subtract test");

        // Act
        var actual = _sut.Subtract(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 5, 25)]
    [InlineData(50, 0, 0)]
    [InlineData(-5, 5, -25)]
    public void Multiply_ShouldMultiplyTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        // Act
        var actual = _sut.Multiply(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    //For diving by 0 exception look "ValueSampleTests"
    [Theory]
    [InlineData(5, 5, 1)]
    [InlineData(15, 5, 3)]
    public void Divide_ShouldDivideTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        // Act
        var actual = _sut.Divide(a, b);

        // Assert
        actual.Should().Be(expected);
    }
}
