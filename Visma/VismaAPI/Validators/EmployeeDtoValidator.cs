using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VismaAPI.Data;
using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPI.Validators;

public sealed class EmployeeDtoValidator : AbstractValidator<EmployeeDto>
{
    private readonly VismaAPIContext context;

    public EmployeeDtoValidator(VismaAPIContext context)
    {
        this.context = context;

        //Only the CEO role has no boss
        RuleFor(e => e.BossId)
            .Null()
            .When(e => e.Role == Role.CEO)
            .WithMessage("CEO must not have a boss.");

        RuleFor(e => e.BossId)
            .NotNull()
            .When(e => e.Role != Role.CEO)
            .WithMessage("Non-CEO employees must have a boss.");

        //All properties, except Boss are required
        RuleFor(e => e.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .NotEqual(e => e.LastName)
            .WithMessage("First name and last name cannot be the same.");

        RuleFor(e => e.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.Birthdate)
            .NotEmpty()
            .Must(BeValidAge)
            .WithMessage("Employee must be between 18 and 70 years old.")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Birthdate cannot be in the future.");

        RuleFor(e => e.EmploymentDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(new DateTime(2000, 1, 1))
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Employment date must be between 2000-01-01 and today.");

        RuleFor(e => e.HomeAddress)
            .NotEmpty();

        RuleFor(e => e.CurrentSalary)
            .SetValidator(new CurrentSalaryValidator());

        RuleFor(e => e.Role)
            .IsInEnum();

        //There can be only 1 employee with CEO role
        RuleFor(e => e)
            .MustAsync(BeTheOnlyCeo)
            .WithMessage("There can only be one CEO.");
    }

    private bool BeValidAge(DateTime birthdate)
    {
        var age = DateTime.Today.Year - birthdate.Year;

        if (birthdate.Date > DateTime.Today.AddYears(-age))
            age--;

        return age >= 18 && age <= 70;
    }

    private async Task<bool> BeTheOnlyCeo(EmployeeDto dto, CancellationToken cancellationToken)
    {
        if (dto.Role != Role.CEO)
            return true;

        var existingCeo = await context.Employees.AnyAsync(emp => emp.Role == Role.CEO, cancellationToken);
        return !existingCeo;
    }
}
