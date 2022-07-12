namespace Customers.Api;

//Here we use IApiMarker, it can be anything not private, for instance "Program" class
//(but then we would need to make internals visible to this project)
//Nevertheless, one of the common practice is to use an interface "IApiMarker" to point to the api
//Its for WebApplicationFactory 
public interface IApiMarker
{
}
