using CalculatorLibrary;
using Xunit.Abstractions;

namespace TestingTechniques.Tests.Unit.Advance;

//Tests run one by one in one class, but in parallel with test in other test classes

//We implement IDisposable in order to make a clean-ups after each test
public class CalculatorDisposeTests : IDisposable
{
    //For every single test execution a new CalculatorTests class will be created.
    //Therefore, only static members wont be created twice or more
    private readonly Calculator _sut = new();
    private readonly Guid _guid = Guid.NewGuid();
    private readonly ITestOutputHelper _outputHelper;
     
    //Setup goes by the constructor
    public CalculatorDisposeTests(ITestOutputHelper outputHelper)
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

    //In order to ignore the test we need to add "Skip" property with a reason (string)
    [Fact(Skip = "We ignore this test for a demo purpose")]
    public void Subtract_ShouldSubtractTwoNumbers_WhenTwoNumberAreIntegers()
    {
        //Act
        var result = _sut.Subtract(10, 5);

        //Assert
        Assert.Equal(5, result);

        _outputHelper.WriteLine("Hello from the Subtract test");
    }

    #region Proof that each class is instantiated every single test

    [Fact]
    public void TestGuid()
    {
        _outputHelper.WriteLine(_guid.ToString());
    }

    [Fact]
    public void TestGuid2()
    {
        _outputHelper.WriteLine(_guid.ToString());
    }

    #endregion

    /// <summary>
    /// Dispose will be called for each test method
    /// </summary>
    public void Dispose()
    {
        _outputHelper.WriteLine("Hello from clean-up");
    }
}