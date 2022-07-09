using CalculatorLibrary;
using Xunit.Abstractions;

namespace TestingTechniques.Tests.Unit.Advance;

//We implement IAsyncLifetime interface to have the async dispose and initialize
public class CalculatorAsyncTests : IAsyncLifetime
{
    //Order:
    //Constructor is called first
    //InitializerAsync is called second
    //Test is called third
    //DisposeAsync is called forth

    private readonly Calculator _sut = new();
    private readonly ITestOutputHelper _outputHelper;

    public CalculatorAsyncTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _outputHelper.WriteLine("Hello from the ctor");
    }

    [Fact]
    public void Add_ShouldAddTwoNumbers_WhenTwoNumberAreIntegers()
    {
        //Act
        var result = _sut.Add(5, 4);

        //Assert
        Assert.Equal(9, result);

        _outputHelper.WriteLine("Hello from the Add test");
    }

    [Fact]
    public void Subtract_ShouldSubtractTwoNumbers_WhenTwoNumberAreIntegers()
    {
        //Act
        var result = _sut.Subtract(10, 5);

        //Assert
        Assert.Equal(5, result);

        _outputHelper.WriteLine("Hello from the Subtract test");
    }

    public async Task DisposeAsync()
    {
        _outputHelper.WriteLine("Hello from async clean-up");
        await Task.Delay(1);
    }

    /// <summary>
    /// This is a method from IAsyncLifetime. It is used to make some initial setups
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        _outputHelper.WriteLine("Hello from async InitializeAsync");
        await Task.Delay(1);
    }
}