using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Customers.Api.Tests.Integration.Collections;

namespace Customers.Api.Tests.Integration.CustomerControllerBasic;

[Collection(CollectionNames.Customer_Basic_Controller_Collection)]
public class GetCustomerControllerBasicTests
{
    private readonly HttpClient _httpClient;

    public GetCustomerControllerBasicTests(WebApplicationFactory<IApiMarker> appFactory)
    {
        _httpClient = appFactory.CreateClient();
    }

    [Fact(Skip = "True tests are in a 'CustomerController' folder")]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        //Act
        var response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Title.Should().Be("Not Found");
        problem.Status.Should().Be(404);
    }

    [Theory(Skip = "True tests are in a 'CustomerController' folder")]
    [MemberData(nameof(Data))]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist2(string guidAsText)
    {
        //Act
        var response = await _httpClient.GetAsync($"customers/{Guid.Parse(guidAsText)}");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public static IEnumerable<object[]> Data { get; } = new[]
    {
        new [] { "8bffe86e-d14c-426c-8ae3-43ee06fbdab0" },
        new [] { "bd4fb277-5b97-40a6-837d-b027998b705e" },
        new [] { "e1608bd8-aeb3-4af9-bb35-eb43ed0d3667" }
    };
}