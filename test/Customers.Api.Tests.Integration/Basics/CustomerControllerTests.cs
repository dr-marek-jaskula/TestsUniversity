using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Customers.Api.Tests.Integration;

//The most important thing about integration testing for webapi is downloading NuGet Package:
//"Microsoft.AspNetCore.Mvc.Testing"
//In this library we have class called "WebApplicationFactory"
//This class will allow us to run integration test without running the application itself
//This class will run web application IN MEMORY, without worrying about where it is, how it can start or run.
//Moreover, it gives us a http client that only that client can call the api that is running.
//When the test is disposed, the api is disposed with it

//We fixture the WebApplicationFactory to have it one per test class
public class CustomerControllerTests : IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime
{
    //Here we use IApiMarker, it can be anything in a referenced project, for instance "Program" class
    //(but then we would need to make internals visible to this project)
    //Nevertheless, one of the common practice is to use an interface "IApiMarker" to point to the api
    //However, we want to create one factory per test class, so we use "IClassFixture" and inject it by constructor
    private readonly HttpClient _httpClient;

    //We use bogus to create fake data
    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.GitHubUsername, "dr-marek-jaskula")
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    //The list to store the ids of created users. Used for later cleanup
    private readonly List<Guid> _createdIds = new();

    public CustomerControllerTests(WebApplicationFactory<IApiMarker> appFactory)
    {
        //Now, using the fixture, we have one instance per test class
        //We do not need to store the factory in a class, so we just use it to create a client and run the app

        //This is a client from a factory, that will only be able to reach in memory api
        _httpClient = appFactory.CreateClient();

        //so we will have test server in memory and client that can call the api
    }

    //In 99% it is enough to just assert: Header, StatusCode, Body

    [Fact(Skip = "Look at CustomerContorller folder and get tests")]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        //Act
        //It is preferred to use the overload of GetAsync and other, that return HtppResponseMessage
        var response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        //Assert
        //do not use EnsureSuccessStatusCode, test should be specific not generic
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        //It is a way how to get response as a text. It is not recommenced, it is better to have it as a JSON
        //var text = await response.Content.ReadAsStringAsync();
        //text.Should().Contain("404");

        //To have the response in a JSON format (but we need to know the structure of a JSON. 
        //In this case the structure is given by "ValidationProblemDetails"
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Title.Should().Be("Not Found");
        problem.Status.Should().Be(404);

        //To validate headers:
        //response.Headers.Location!.ToString().Should().Be("");
    }

    [Theory(Skip = "Look at CustomerContorller folder and get tests")] 
    [MemberData(nameof(Data))]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist2(string guidAsText)
    {
        //Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

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

    [Fact(Skip = "Look at CustomerContorller folder and create tests")]
    public async Task Create_ReturnsCreated_WhenCustomerIsCreated()
    {
        //Arrange
        var customer = _customerGenerator.Generate();

        //Act
        var response = await _httpClient.PostAsJsonAsync("customers ", customer);

        //Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(customer);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _createdIds.Add(customerResponse!.Id);
    }

    //Now I do not care about it
    public Task InitializeAsync() => Task.CompletedTask;

    //Will clean the record we insert into database
    public async Task DisposeAsync()
    {
        foreach (var createdId in _createdIds)
            await _httpClient.DeleteAsync($"customers/{createdId}");
    }
}