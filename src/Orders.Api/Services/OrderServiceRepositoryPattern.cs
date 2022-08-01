using AutoMapper;
using Microsoft.AspNetCore.Enums;
using Microsoft.EntityFrameworkCore;
using Orders.Api.DbModels;
using Orders.Api.Models;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Models.QueryObjects;
using Orders.Api.Repositories;
using System.Linq.Expressions;

namespace Orders.Api.Services;

public class OrderServiceRepositoryPattern : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderServiceRepositoryPattern(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto> GetById(int id)
    {
        var order = await _orderRepository.GetById(id);

        var result = _mapper.Map<OrderDto>(order);

        return result;
    }

    public async Task Update(int id, UpdateOrderDto dto)
    {
        await _orderRepository.Update(id, dto);
    }

    public async Task<PageResult<OrderDto>> GetAll(OrderQuery query)
    {
        var baseQuery = _orderRepository.GetAll(query);

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

        await _orderRepository.Delete(orderToDelete);
    }

    public async Task Create(CreateOrderDto dto)
    {
        var order = _mapper.Map<Order>(dto);

        await _orderRepository.Create(order);
    }
}