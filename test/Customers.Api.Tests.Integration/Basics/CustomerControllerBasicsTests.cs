using System.Collections;
using System.Net;

namespace Customers.Api.Tests.Integration;

//1. Constructor is called
//2. InitializeAsync is called
//3. Test runs
//4. Cleanup async (DisposeAsync)
//5. Clean sync (Dispose) (for each method)

//6. Again constructor, initializeAsync, RunTest, Cleanup async, Cleanup sync (and so on for each test)

//IAsyncLifetime for setup the async code
//IDisposeable is for sync cleanup
public class CustomerControllerBasicsTests : IAsyncLifetime, IDisposable
{
    //This client will be able to call the running api
    private readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:5001")
    };

    public CustomerControllerBasicsTests()
    {
        //Setup code here
    }

    //We can skip the test
    [Fact(Skip = "This is a way without WebApplicationFactory")]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        //Act
        var response = await httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory(Skip = "This is a way without WebApplicationFactory")] //We can skip all the tests on a theory level
    [ClassData(typeof(ClassData))]
    //[MemberData(nameof(Data))]
    //[InlineData("8bffe86e-d14c-426c-8ae3-43ee06fbdab0")]
    //[InlineData("bd4fb277-5b97-40a6-837d-b027998b705e")]
    //[InlineData("e1608bd8-aeb3-4af9-bb35-eb43ed0d3667")]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist2(string guidAsText)
    {
        //Act
        var response = await httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    //Data from a method or a property
    public static IEnumerable<object[]> Data { get; } = new[]
    {
        new [] { "8bffe86e-d14c-426c-8ae3-43ee06fbdab0" },
        new [] { "bd4fb277-5b97-40a6-837d-b027998b705e" },
        new [] { "e1608bd8-aeb3-4af9-bb35-eb43ed0d3667" }
    };

    //Async setup
    public async Task InitializeAsync()
    {
        await Task.Delay(1000);
    }

    //async cleanup
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    //Sync cleanup
    public void Dispose()
    {
        //Any cleanup code
    }
}

//Data from a class
public class ClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "8bffe86e-d14c-426c-8ae3-43ee06fbdab0" };
        yield return new object[] { "bd4fb277-5b97-40a6-837d-b027998b705e" };
        yield return new object[] { "e1608bd8-aeb3-4af9-bb35-eb43ed0d3667" };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}