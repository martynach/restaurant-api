using FluentValidation;
using RestaurantApi3.Entities;

namespace RestaurantApi3.Dtos.Validators;

public class RestaurantQueryValidator: AbstractValidator<RestaurantQuery>
{
    public RestaurantQueryValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).Custom((value, context) =>
        {
            var allowedPageSizes = new int[] { 5, 10, 15 };
            if (!allowedPageSizes.Contains(value))
            {
                context.AddFailure("Allowed items per page are 5, 10, 15");
            }
        });

        // RuleFor(query => query.SortBy).Custom((value, context) =>
        // {
        //     var allowedSortByColumns = new string[]
        //         { nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.Description) };
        //     if (value != null && !allowedSortByColumns.Contains(value))
        //     {
        //         var message = "Allowed sortBy values: ";
        //         foreach (var val in allowedSortByColumns)
        //         {
        //             message += val + " ";
        //         }
        //         context.AddFailure(message);
        //
        //     }
        // });
        
        var allowedSortByColumns = new string[]
            { nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.Description) };
        RuleFor(query => query.SortBy)
            .Must(sortBy => String.IsNullOrEmpty(sortBy) || allowedSortByColumns.Contains(sortBy))
            .WithMessage($"sortBy is optional or must be equal: {string.Join(" ", allowedSortByColumns)}");

    }
}