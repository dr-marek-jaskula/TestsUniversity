using CalculatorLibrary;
using System.Collections;

namespace TestingTechniques.Tests.Unit.Advance;

//Here we will show two different ways of implementing parameters into Theory test methods
//Third one will be presented in "CalculatorTheoryFromFileTests"

public class CalculatorTheoryTests
{
    private readonly Calculator _sut = new();
     
    public CalculatorTheoryTests()
    {
    }

    //1. First approach is to use "MemberData" that uses a method to provide inputs
    //We specify the name of a method (in a "nameof") and write a static method with "IEnumerable<object[]>" return type
    public static IEnumerable<object[]> AddTestData()
    {
        yield return new object[] { 5, 5, 10 };
        yield return new object[] { -5, 5, 0 };
        yield return new object[] { -15, -5, -20 };
    }

    //We can also to this without using "yield return" but just (not preferred):
    public static IEnumerable<object[]> AddTestData2() =>
    new List<object[]>
    {
        new object[] { 5, 5, 10 },
        new object[] { -5, 5, 0 },
        new object[] { -15, -5, -20 }
    };

    [Theory]
    [MemberData(nameof(AddTestData))]
    public void Add_ShouldAddTwoNumbers_WhenTheNumbersAreValidIntegers(int a, int b, int expectedResult)
    {
        // Act
        var actualResult = _sut.Add(a, b);

        // Assert
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(AddTestData2))]
    public void Add_ShouldAddTwoNumbers_WhenTheNumbersAreValidIntegers2(int a, int b, int expectedResult)
    {
        // Act
        var actualResult = _sut.Add(a, b);

        // Assert
        actualResult.Should().Be(expectedResult);
    }

    //2. The next option is when we for instance have many parameters or we want to dynamically create them
    //Then we use "ClassData" approach with "typeof" of a class that gathers our data 

    [Theory]
    [ClassData(typeof(CalculatorSubtractTestData))]
    public void Subtract_ShouldSubtractTwoNumbers_WhenTheNumbersAreValidIntegers(
        int firstNumber, int secondNumber, int expectedResult)
    {
        // Act
        var actualResult = _sut.Subtract(firstNumber, secondNumber);

        // Assert
        actualResult.Should().Be(expectedResult);
    }

    //3. Third approach is presented in "CalculatorTheoryFromFileTests"
}

//This class needs to implement "IEnumerable<object[]>" interface
public class CalculatorSubtractTestData : IEnumerable<object[]>
{
    //The enumerator needs to have "yield return" as it is a enumerator
    //Therefore, therefore the data is given dynamically
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 5, 5, 0 };
        yield return new object[] { -5, -5, 0 };
        yield return new object[] { -15, -5, -10 };
        yield return new object[] { 15, 5, 10 };
        yield return new object[] { 5, 10, -5 };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}