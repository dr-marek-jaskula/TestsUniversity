namespace TestingTechniques;

//This is a very important interface to make classes testable
//It is a common thing to see this in a project (usually as a singleton)

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime DateTimeNow => DateTime.Now;
}

public interface IDateTimeProvider
{
    public DateTime DateTimeNow { get; }
}
