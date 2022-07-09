namespace TestingTechniques.Tests.Unit.Master;

//This is a way how to test datetime and similar code.
//0. Create an IDateTimeProvider interface and one implementation
//1. To make our code testable, we should add "IDateTimeProvider" to our code
//2. Then make a mock for this IDateTimeProvider in a test class

public class GreeterTests
{
    private readonly Greeter _sut;
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    public GreeterTests()
    {
        _sut = new Greeter(_dateTimeProvider);
    }

    [Fact]
    public void GenerateGreetMessage_ShouldSayGoodEvening_WhenItsEvening()
    {
        // Arrange
        _dateTimeProvider.DateTimeNow.Returns(new DateTime(2020, 1, 1, 20, 0, 0));

        // Act
        var actual = _sut.GenerateGreetMessage();

        // Assert
        actual.Should().Be("Good evening");
    }

    [Fact]
    public void GenerateGreetMessage_ShouldSayGoodMorning_WhenItsMorning()
    {
        // Arrange
        _dateTimeProvider.DateTimeNow.Returns(new DateTime(2020, 1, 1, 10, 0, 0));

        // Act
        var actual = _sut.GenerateGreetMessage();

        // Assert
        actual.Should().Be("Good morning");
    }

    [Fact]
    public void GenerateGreetMessage_ShouldSayGoodAfternoon_WhenItsAfternoon()
    {
        // Arrange
        _dateTimeProvider.DateTimeNow.Returns(new DateTime(2020, 1, 1, 15, 0, 0));

        // Act
        var actual = _sut.GenerateGreetMessage();

        // Assert
        actual.Should().Be("Good afternoon");
    }
}