using AutoMapper;
using Microsoft.AspNetCore.Enums;
using Microsoft.EntityFrameworkCore;
using Orders.Api.DbModels;
using Orders.Api.Exceptions;
using Orders.Api.Models;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Models.QueryObjects;
using System.Linq.Expressions;

namespace Orders.Api.Services;
public interface IOrderService
{
    Task<OrderDto> GetById(int id);

    Task<PageResult<OrderDto>> GetAll(OrderQuery query);

    Task Delete(int id);

    Task Update(int id, UpdateOrderDto dto);

    Task Create(CreateOrderDto dto);

}

public class OrderService : IOrderService
{
    private readonly MyDbContext _dbContex;
    private readonly IMapper _mapper;

    public OrderService(MyDbContext dbContex, IMapper mapper)
    {
        _dbContex = dbContex;
        _mapper = mapper;
    }

    public async Task<OrderDto> GetById(int id)
    {
        var order = await _dbContex.Orders.FindAsync(id);

        if (order is null)
            throw new NotFoundException("Order not found");

        var result = _mapper.Map<OrderDto>(order);
        return result;
    }

    public async Task Update(int id, UpdateOrderDto dto)
    {
        var order = await _dbContex.Orders.FindAsync(id);

        if (order is null)
            throw new NotFoundException("Order not found");

        order.Amount = dto.Amount;

        await _dbContex.SaveChangesAsync();
    }

    public async Task<PageResult<OrderDto>> GetAll(OrderQuery query)
    {
        var baseQuery = _dbContex.Orders
                  .AsNoTracking()
                  .Where(o => query.SearchPhrase == null
                                || o.Name.ToLower().Contains(query.SearchPhrase.ToLower()));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Order, object>>>
                {
                    { nameof(Order.Name), o => o.Name},
                    { nameof(Order.Amount), o => o.Amount },
                };

            var selectedColumn = columnsSelector[query.SortBy];

            baseQuery = query.SortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var orders = await baseQuery
            .Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToListAsync();

        int totalItemsCount = baseQuery.Count();

        var ordersDtos = _mapper.Map<List<OrderDto>>(orders);

        var result = new PageResult<OrderDto>(ordersDtos, totalItemsCount, query.PageSize, query.PageNumber);

        return result;
    }

    public async Task Delete(int id)
    {
        var orderToDelete = new Order() { Id = id };
        var entry = _dbContex.Orders.Attach(orderToDelete);
        entry.State = EntityState.Deleted;
        try
        {
            await _dbContex.SaveChangesAsync();
        }
        catch
        {
            throw new NotFoundException($"Order with id = {id} not found");
        }
    }

    public async Task Create(CreateOrderDto dto)
    {
        var order = _mapper.Map<Order>(dto);

        _dbContex.Orders.Add(order);

        await _dbContex.SaveChangesAsync();
    }
}