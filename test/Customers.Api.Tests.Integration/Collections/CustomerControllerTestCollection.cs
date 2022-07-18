namespace Customers.Api.Tests.Integration.Collections;

//Share the context of a CustomerApiFactory

[CollectionDefinition(CollectionNames.Customer_Controller_Collection)]
public class CustomerControllerTestCollection : ICollectionFixture<CustomerApiFactory>
{
}