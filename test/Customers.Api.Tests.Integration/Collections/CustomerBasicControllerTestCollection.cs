using Microsoft.AspNetCore.Mvc.Testing;

namespace Customers.Api.Tests.Integration.Collections;

//Here we share the context
//So the collection will be now a set of test classes that are decorated by attribute 
//[Collection("CustomerControllerCollection")
//So one instance of a WebApplicationFactory will be shared for all of them, and they will run one by one

[CollectionDefinition(CollectionNames.Customer_Basic_Controller_Collection)]
public class CustomerBasicControllerTestCollection : ICollectionFixture<WebApplicationFactory<IApiMarker>>
{

}