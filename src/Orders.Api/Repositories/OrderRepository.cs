using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Models.QueryObjects;
using Orders.Api.DbModels;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Exceptions;

namespace Orders.Api.Repositories;

public interface IOrderRepository
{
    Task<Order> GetById(int id);

    IQueryable<Order> GetAll(OrderQuery query);

    Task Delete(Order orderToDelete);

    Task Update(int id, UpdateOrderDto dto);

    Task Create(Order order);
}

public class OrderRepository : IOrderRepository
{
    private readonly MyDbContext _dbContext;

    public OrderRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(Order order)
    {
        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Order orderToDelete)
    {
        var entry = _dbContext.Orders.Attach(orderToDelete);

        entry.State = EntityState.Deleted;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            throw new NotFoundException($"Order with id = {orderToDelete.Id} not found");
        }
    }

    public IQueryable<Order> GetAll(OrderQuery query)
    {
        return _dbContext.Orders
                  .AsNoTracking()
                  .Where(o => query.SearchPhrase == null
                                || o.Name.ToLower().Contains(query.SearchPhrase.ToLower()));
    }

    public async Task<Order> GetById(int id)
    {
        var order = await _dbContext.Orders.FindAsync(id);

        if (order is null)
            throw new NotFoundException("Order not found");

        return order;
    }

    public async Task Update(int id, UpdateOrderDto dto)
    {
        var order = await _dbContext.Orders.FindAsync(id);

        if (order is null)
            throw new NotFoundException("Order not found");

        order.Amount = dto.Amount;

        await _dbContext.SaveChangesAsync();
    }
}
