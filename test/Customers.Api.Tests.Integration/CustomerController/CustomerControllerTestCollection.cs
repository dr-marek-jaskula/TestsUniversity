using Customers.Api.Tests.Integration.CustomerController;

namespace Customers.Api.Tests.Integration.CustomerControllerBasic;

//Share the context of a CustomerApiFactory

[CollectionDefinition("CustomerControllerTestCollection")]
public class CustomerControllerTestCollection : ICollectionFixture<CustomerApiFactory>
{
}