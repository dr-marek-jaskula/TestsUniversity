using FluentValidation;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Models.QueryObjects;

namespace Orders.Api.Models.Validators;
public class OrderQueryValidator : AbstractValidator<OrderQuery>
{
    //Pagination parameters

    private readonly int[] _allowedPageSizes = new[] { 5, 10, 15 };
    private readonly string[] _allowedSortByColumnNames = { nameof(OrderDto.Name), nameof(OrderDto.Amount) };

    public OrderQueryValidator()
    {
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize).Custom((value, context) =>
        {
            if (!_allowedPageSizes.Contains(value))
                context.AddFailure("PageSize", $"PageSize must be in [{string.Join(",", _allowedPageSizes)}]");
        });

        RuleFor(r => r.SortBy)
            .Must(value => string.IsNullOrEmpty(value) || _allowedSortByColumnNames.Contains(value))
            .WithMessage($"Sort by is optional or must be in [{string.Join(",", _allowedSortByColumnNames)}]");
    }
}