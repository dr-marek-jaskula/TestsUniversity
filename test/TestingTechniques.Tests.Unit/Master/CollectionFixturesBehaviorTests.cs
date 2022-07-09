using Xunit.Abstractions;

namespace TestingTechniques.Tests.Unit.Master;

//In this file we have to test classes: "CollectionFixturesBehaviorTests" and "CollectionFixturesBehaviorTestsAgain"
//We will share the common fixed context between both of them

//1. We create a "TestCollectionFixture" 
//2. We add attributes "Collection" with same name as in "TestCollectionFixture" to all classes we want to have shared fixture

[Collection("My custom collection fixture")]
public class CollectionFixturesBehaviorTests1
{
    private readonly ITestOutputHelper _testOutputHelper;

    //So the context of this field will be shared across each test
    private readonly MyClassFixture _fixture;

    public CollectionFixturesBehaviorTests1(ITestOutputHelper testOutputHelper, MyClassFixture fixture)
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

[Collection("My custom collection fixture")]
public class CollectionFixturesBehaviorTests2
{
    private readonly ITestOutputHelper _testOutputHelper;

    //So the context of this field will be shared across each test
    private readonly MyClassFixture _fixture;

    public CollectionFixturesBehaviorTests2(ITestOutputHelper testOutputHelper, MyClassFixture fixture)
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