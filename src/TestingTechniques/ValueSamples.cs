namespace TestingTechniques;

public class ValueSamples
{
    public string FullName = "Marek Jaskuła";

    public int Age = 30;

    public DateOnly DateOfBirth = new(1992, 5, 13);

    public User AppUser = new()
    {
        FullName = "Marek Jaskuła",
        Age = 20,
        DateOfBirth = new(1992, 5, 13)
    };

    public IEnumerable<User> Users = new[]
    {
        new User()
        {
            FullName = "Marek Jaskuła",
            Age = 30,
            DateOfBirth = new(2000, 6, 9)
        },
        new User()
        {
            FullName = "Michał Kowalski",
            Age = 39,
            DateOfBirth = new(1986, 3, 4)
        },
        new User()
        {
            FullName = "Anna Andruszkiewicz",
            Age = 42,
            DateOfBirth = new(1979, 12, 6)
        }
    };

    public IEnumerable<int> Numbers = new[] { 2, 3, 5, 8 };

    public event EventHandler ExampleEvent;

    internal int InternalSecretNumber = 77;

    public virtual void RaiseExampleEvent()
    {
        ExampleEvent(this, EventArgs.Empty);
    }
}
