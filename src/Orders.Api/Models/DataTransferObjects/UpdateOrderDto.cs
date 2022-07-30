namespace Orders.Api.Models.DataTransferObjects;

public record class UpdateOrderDto
(
    int Amount,
    int? ProductId = null
);