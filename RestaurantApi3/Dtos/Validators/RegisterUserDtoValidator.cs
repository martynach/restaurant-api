using FluentValidation;
using RestaurantApi3.Entities;

namespace RestaurantApi3.Dtos.Validators;

public class RegisterUserDtoValidator: AbstractValidator<RegisterUserDto>
{

    public RegisterUserDtoValidator(RestaurantDbContext _dbcontext)
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(u => u.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Equal(u => u.Password);

        RuleFor(u => u.Email)
            .Custom((value, context) =>
            {
                var emailExists = _dbcontext.Users.Any(u => u.Email == value);
                if (emailExists)
                {
                    context.AddFailure("Email", $"Email: '{value}' already taken by another user.");
                }
            });


    }
    
}