using FluentValidation;

namespace VismaAPI.Validators;

internal sealed class CurrentSalaryValidator : AbstractValidator<decimal>
{
    public CurrentSalaryValidator()
    {
        RuleFor(salary => salary)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Salary must be non-negative.");
    }
}
