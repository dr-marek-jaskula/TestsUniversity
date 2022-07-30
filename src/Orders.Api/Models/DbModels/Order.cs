namespace Orders.Api.DbModels;

public class Order
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
    public DateTime Deadline { get; set; }
}