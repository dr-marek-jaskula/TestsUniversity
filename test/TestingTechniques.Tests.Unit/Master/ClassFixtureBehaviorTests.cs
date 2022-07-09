using Xunit.Abstractions;

namespace TestingTechniques.Tests.Unit.Master;

//Tests by default runs one by one in one class (one instance per test)
//However, it runs in parallel with test in other test classes (other collections)

//Each test class is by default treated as a collection.
//Test in a collection by default does not run in parallel
//So test in different collection (like by default in different test classes) run in parallel

//In order to have a fixture behavior (something that is common for tests, like a guid), we can implement
//IClassFixture<TypeOfClassThatWillBeFixed> 
//Also the constructor of TypeOfClassThatWillBeFixed will be shared for each test
//We can create a private filed and inject it by the constructor (the dependency injection will be done by xUnit)
public class ClassFixtureBehaviorTests : IClassFixture<MyClassFixture>
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly ITestOutputHelper _testOutputHelper;

    //So the context of this field will be shared across each test
    private readonly MyClassFixture _fixture;

    public ClassFixtureBehaviorTests(ITestOutputHelper testOutputHelper, MyClassFixture fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Fact]
    public async Task ExampleTest1()
    {
        //This _fixture will be the same as in other test methods
        _testOutputHelper.WriteLine($"The Guid was: {_fixture.Id}");
        await Task.Delay(2000);
    }

    [Fact]
    public async Task ExampleTest2()
    {
        _testOutputHelper.WriteLine($"The Guid was: {_fixture.Id}");
        await Task.Delay(2000);
    }
}

//Summary:
//1. The first what is called is the constructor of the fixture
//2. Secondly the test class constructor is being called
//3. A certain test runs
//4. Test class constructor is called again
//5. Next test runs
// ...
// The dispose method of a fixture is called for the test class that has a fixture