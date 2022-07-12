using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using System.Net;
using System.Net.Http.Json;

namespace Customers.Api.Tests.Integration.CustomerController;

//This should in a other collection or use IClassFixture or made some cleanup after other tests
//[Collection("CustomerControllerTestCollection")]
public class GetAllCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGitHubUser)
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    public GetAllCustomerControllerTests(CustomerApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllCustomers_WhenCustomersExist()
    {
        //Arrange
        var customer = _customerGenerator.Generate();
        var createdResponse = await _client.PostAsJsonAsync("customers", customer);
        var createdCustomer = await createdResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        //Act
        var response = await _client.GetAsync("customers");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customersResponse = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
        customersResponse!.Customers.Single().Should().BeEquivalentTo(createdCustomer);

        //Cleanup
        //so the next test will check if the user is not added if the table was empty
        await _client.DeleteAsync($"customers/{createdCustomer!.Id}");
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyResult_WhenNoCustomersExist()
    {
        //Act
        var response = await _client.GetAsync("customers");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customersResponse = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
        customersResponse!.Customers.Should().BeEmpty();
    }
}