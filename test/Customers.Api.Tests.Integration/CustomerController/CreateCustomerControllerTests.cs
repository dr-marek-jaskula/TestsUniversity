using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController;

[Collection("CustomerControllerTestCollection")]
public class CreateCustomerControllerTests
{
    //This apiFactory is injected by xUnit. 
    //One instance per whole collection
    private readonly CustomerApiFactory _apiFactory;

    //Also a http client is required
    private readonly HttpClient _httpClient;

    //Bogus fake data
    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGitHubUser);

    public CreateCustomerControllerTests(CustomerApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _httpClient = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldCreateUser_WhenDataIsValid()
    {
        //Arrange 
        var customer = _customerGenerator.Generate();

        //Act
        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        //Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(customer);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be($"http://localhost/customers/{customerResponse!.Id}");
    }

    [Fact]
    public async Task Create_ShouldReturnValidationError_WhenEmailIsInvalid()
    {
        //Arrange
        const string invalidEmail = "dfdfregrfs";
        //"Clone" will give other Faker with the same rules. It is done in order not to mutate the Faker instance
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail)
            .Generate();

        //Act
        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error!.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
    }

    [Fact]
    public async Task Create_ShouldReturnValidationError_WhenGitHubUserDoesNotExist()
    {
        //Arrange
        const string invalidGitHubUser = "dfdfregrfs";
        //"Clone" will give other Faker with the same rules. It is done in order not to mutate the Faker instance
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, invalidGitHubUser)
            .Generate();

        //Act
        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error!.Errors["GitHubUsername"][0].Should().Be($"There is no GitHub user with username {invalidGitHubUser}");
    }

    //Tests for a throttled user 
    //Api we test handlers the throttle user in a bad way, but nevetherless we will write a code to deal with it
    [Fact]
    public async Task Create_ShouldReturnInternalServerError_WhenGitHubIsThrottled()
    {
        //Arrange 
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ThrottledUser)
            .Generate();

        //For authorization/authentication
        //We can do like this (be aware if there is a space or if Bearer is from capital letter
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", myToken);

        //Act
        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}