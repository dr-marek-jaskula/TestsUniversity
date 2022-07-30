using Microsoft.AspNetCore.Enums;

namespace Orders.Api.Models.QueryObjects;

public record class OrderQuery
(
    string SearchPhrase,
    int PageNumber,
    int PageSize,
    string SortBy,
    SortDirection SortDirection
);