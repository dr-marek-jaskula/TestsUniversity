using CalculatorLibrary;
using TestingTechniques.Tests.Unit.CustomTools;

namespace TestingTechniques.Tests.Unit.Advance;

//Here the approach of using the custom class (that inherits from "DataAttribute") will be demonstrated.
//The class is in "CustomTools" and it is "JsonFileData". This class is an attribute

public class CalculatorTheoryFromFileTests
{
    private readonly Calculator _sut = new();

    [Theory]
    //This is a relative path to bin.
    //Remember to mark the json file as "CopyIfNewer" (will copy to /bin/Data directory)
    [JsonFileData("Data/AddCalculatorData.json")] 
    public void Add_ShouldAddTwoNumbers_WhenTheNumbersAreValidIntegers(int a, int b, int expectedResult)
    {
        // Act
        var actualResult = _sut.Add(a, b);

        // Assert
        actualResult.Should().Be(expectedResult);
    }
}