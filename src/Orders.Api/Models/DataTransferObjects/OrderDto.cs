namespace Orders.Api.Models.DataTransferObjects;

public record class OrderDto
(
    int Id,
    string Name,
    int Amount,
    DateTime Deadline
);