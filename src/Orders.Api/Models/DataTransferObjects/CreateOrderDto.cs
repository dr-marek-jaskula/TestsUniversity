namespace Orders.Api.Models.DataTransferObjects;

public record class CreateOrderDto
(
    string Name,
    int Amount,
    DateTime Deadline
);