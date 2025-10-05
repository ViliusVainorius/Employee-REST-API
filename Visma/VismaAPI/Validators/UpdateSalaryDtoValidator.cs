using FluentValidation;
using VismaAPI.Models;

namespace VismaAPI.Validators;

public sealed class UpdateSalaryDtoValidator : AbstractValidator<UpdateSalaryDto>
{
    public UpdateSalaryDtoValidator()
    {
        RuleFor(x => x.NewSalary)
            .SetValidator(new CurrentSalaryValidator());
    }
}
