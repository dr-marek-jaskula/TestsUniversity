using CalculatorLibrary;

namespace TestingTechniques.Tests.Unit.Basics;

//This file was introduced to learn how to assert: properties (public, internal), exceptions, collection, objects, etc.

//To examine how to test void method,
//For instance a log, go to: GetAllAsync_ShouldLogMessages_WhenInvoked() in UserServicesTests

public class ValueSamplesTests
{
    private readonly ValueSamples _sut = new();

    //Assert strings
    [Fact]
    //[Fact(Skip = "We ignore this test for a demo purpose")] //We can skip some test if we want to. We need to give a reason
    //If we want to underline the trait (for instance "Category", "UnitTest") for filtering purpose we can do this
    [Trait("Category", "UnitTest")]
    public void StringAssertionExample()
    {
        var fullName = _sut.FullName;

        fullName.Should().Be("Marek Jaskuła");
        fullName.Should().NotBeEmpty();
        fullName.Should().StartWith("Marek");
        fullName.Should().EndWith("Jaskuła");
    }

    //Assert numbers
    [Fact]
    public void NumberAssertionExample()
    {
        var age = _sut.Age;

        age.Should().Be(30);
        age.Should().BePositive();
        age.Should().BeGreaterThan(28);
        age.Should().BeLessThanOrEqualTo(32);
        age.Should().BeInRange(29, 32);
    }

    //Assert dates
    [Fact]
    public void DateAssertionExample()
    {
        var dateOfBirth = _sut.DateOfBirth;

        dateOfBirth.Should().Be(new(1992, 5, 13));
        dateOfBirth.Should().BeAfter(new(1991, 5, 22));
        dateOfBirth.Should().BeBefore(new(1993, 2, 3));
    }

    //Assert objects
    [Fact]
    public void ObjectAssertionExample()
    {
        var expected = new User()
        {
            FullName = "Marek Jaskuła",
            Age = 20,
            DateOfBirth = new(1992, 5, 13)
        };

        //This is a reference type
        var user = _sut.AppUser;

        //To compare values of the reference types (that are not strings)
        //We need to check the equivalence
        user.Should().BeEquivalentTo(expected);
    }

    //Assert enumerable of reference type
    [Fact]
    public void EnumerableAssertionExample()
    {
        var expected = new User()
        {
            FullName = "Marek Jaskuła",
            Age = 30,
            DateOfBirth = new(2000, 6, 9)
        };

        //To safety cast the enumerable to an array
        var users = _sut.Users.As<User[]>();

        //To check the reference type we need to use "ContainEquivalentOf"
        users.Should().ContainEquivalentOf(expected);
        users.Should().HaveCount(3);
        users.Should().Contain(x => x.FullName.StartsWith("Marek") && x.Age > 5);
    }

    //Assert enumerable of value type
    [Fact]
    public void EnumerableValueTypesAssertionExample()
    {
        var numbers = _sut.Numbers.As<int[]>();

        //For value types we just need to:
        numbers.Should().Contain(5);
    }

    //Assert throwing exception
    [Fact]
    public void ExceptionThrownAssertionExample()
    {
        var calculator = new Calculator();

        //In order to deal with the method that should throw an exception we wrap it in a action
        Action result = () => calculator.Divide(1, 0);

        result.Should()
            .Throw<DivideByZeroException>()
            .WithMessage("Attempted to divide by zero.");
    }

    //Assert rising an event
    [Fact]
    public void EventRaisedAssertionExample()
    {
        //We create a monitor that monitor events that can rise and stores the information about them
        var monitorSubject = _sut.Monitor();

        _sut.RaiseExampleEvent();

        monitorSubject.Should().Raise("ExampleEvent");
    }

    //Testing internal member (look configuration details in "ReadMe" or to the project file
    [Fact]
    public void TestingInternalMembersExample()
    {
        var number = _sut.InternalSecretNumber;

        number.Should().Be(77);
    }
}