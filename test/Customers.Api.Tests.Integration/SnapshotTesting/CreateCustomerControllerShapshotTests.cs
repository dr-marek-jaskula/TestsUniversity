using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Customers.Api.Tests.Integration.Collections;
using System.Net.Http.Json;

namespace Customers.Api.Tests.Integration.CustomerController;

//1. Install Verify.Xunit NuGet Package
//2. Decorate the class with an attribute [UsesVerify]
//For snapshots we need fixed test data, therefore we need to use Randomizer.Seed for the Bogus package
//3. Add to the CustomerApiFactory the constructor with:
/*
    public CustomerApiFactory()
    {
        Randomizer.Seed = new Random(420); 
        VerifierSettings.ScrubInlineGuids();
    } 
*/
//4. Add "await Verify(customerReponse).UseDirectory("Snapshots");" line in assertions section (we can use other folder)
//5. Run the test. It will fail, but it will generate the response.
//6. If the response is ok, we accept it, we will use its snapshot for further tests.
//!! the snapshot is scrubbing the guid (so Guid_1 says this is just a random guid) and the date (DateTime_1 says this is just a random date)
//The generated response is "received" one:
//"CreateCustomerControllerShapshotTests.Create_ShouldCreateUser_WhenDataIsValid.received"
//The accepted one is:
//"CreateCustomerControllerShapshotTests.Create_ShouldCreateUser_WhenDataIsValid.verified"
//We should include these received and verified to git

//This package can be used for instance to verifying:
//Image
//pdf
//auto generated code
//html

[UsesVerify]
[Collection(CollectionNames.Customer_Controller_Collection)]
public class CreateCustomerControllerShapshotTests
{
    //This apiFactory is injected by xUnit.
    private readonly CustomerApiFactory _apiFactory;
    private readonly HttpClient _httpClient;

    //Bogus fake data
    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGitHubUser);

    public CreateCustomerControllerShapshotTests(CustomerApiFactory apiFactory)
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
        await Verify(customerResponse)
            .UseDirectory("Snapshots");
    }
}