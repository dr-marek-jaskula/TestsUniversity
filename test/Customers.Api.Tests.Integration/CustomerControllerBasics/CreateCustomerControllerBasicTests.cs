using Customers.Api.Contracts.Responses;
using System.Net;
using Bogus;
using Customers.Api.Contracts.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace Customers.Api.Tests.Integration.CustomerControllerBasic;

[Collection("CustomerControllerBasicCollection")]
public class CreateCustomerControllerBasicTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;

    //We use bogus to create fake data
    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.GitHubUsername, "dr-marek-jaskula")
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    //The list to store the ids of created users. Used for later cleanup
    private readonly List<Guid> _createdIds = new();

    public CreateCustomerControllerBasicTests(WebApplicationFactory<IApiMarker> appFactory)
    {
        _httpClient = appFactory.CreateClient();
    }

    [Fact(Skip = "True tests are in a 'CustomerController' folder")]
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