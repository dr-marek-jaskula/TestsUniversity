using Microsoft.AspNetCore.Mvc;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Models.QueryObjects;
using Orders.Api.Services;

namespace Orders.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get an order specified by its id
    /// </summary>
    /// <param name="id">Order's Id</param>
    /// <returns>Order</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Get([FromRoute] int id)
    {
        var order = await _orderService.GetById(id);
        return Ok(order);
    }

    /// <summary>
    /// Get paginated orders. For SortBy choose "Name" or "Amount"
    /// </summary>
    /// <param name="query">Pagination parameters</param>
    /// <returns>Collection of Orders</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll([FromQuery] OrderQuery query)
    {
        var orderDtos = await _orderService.GetAll(query);
        return Ok(orderDtos);
    }

    /// <summary>
    /// Delete an order specified by its id
    /// </summary>
    /// <param name="id">Order's Id</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _orderService.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// Creates an order in a database
    /// </summary>
    /// <param name="dto">Data to create an Order</param>
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        await _orderService.Create(dto);
        return Created($"/api/order/{2}", null);
    }

    /// <summary>
    /// Update an order in the database
    /// </summary>
    /// <param name="dto">Updated Order</param>
    /// <param name="id">Order's Id</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateOrderDto dto, [FromRoute] int id)
    {
        await _orderService.Update(id, dto);
        return Ok();
    }
}