using Microsoft.EntityFrameworkCore;
using VismaAPI.Data;
using VismaAPI.Domain;
using VismaAPI.Models;
using VismaAPI.Validators;

namespace VismaAPIUnitTests.Validators;

[TestClass]
public class EmployeeDtoValidatorTests
{
    private readonly EmployeeDtoValidator _validator;

    public EmployeeDtoValidatorTests()
    {
        var options = new DbContextOptionsBuilder<VismaAPIContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        var context = new VismaAPIContext(options);
        _validator = new EmployeeDtoValidator(context);
    }

    private EmployeeDto CreateValidEmployeeDto()
    {
        return new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-25),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street 123",
            CurrentSalary = 1000,
            Role = Role.Employee
        };
    }

    // All properties, except for Boss are required
    [TestMethod]
    public async Task Validate_MissingRequiredFields_ShouldHaveValidationErrors()
    {
        var dto = CreateValidEmployeeDto();

        dto.FirstName = "";

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any());
    }

    // Only the CEO role has no boss
    [TestMethod]
    public async Task Validate_CEOWithBoss_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.Role = Role.CEO;
        dto.BossId = 5;

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "BossId"));
    }

    // There can be only 1 employee with CEO role
    [TestMethod]
    public async Task Validate_MultipleCEOs_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.Role = Role.CEO;

        using (var context = new VismaAPIContext(new DbContextOptionsBuilder<VismaAPIContext>()
            .UseInMemoryDatabase("TestDb_CEO")
            .Options))
        {
            context.Employees.Add(new Employee
            {
                FirstName = "Jane",
                LastName = "Smith",
                Birthdate = DateTime.Now.AddYears(-40),
                EmploymentDate = DateTime.Now.AddYears(-10),
                HomeAddress = "Street",
                CurrentSalary = 2000,
                Role = Role.CEO
            });
            await context.SaveChangesAsync();

            var validator = new EmployeeDtoValidator(context);
            var result = await validator.ValidateAsync(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(a => a.ErrorMessage.Contains("There can only be one CEO")));
        }
    }

    // FirstName and LastName cannot be longer than 50 characters
    [TestMethod]
    public async Task Validate_FirstNameTooLong_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.FirstName = new string('A', 51);

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "FirstName"));
    }

    // FirstName != LastName
    [TestMethod]
    public async Task Validate_FirstNameEqualsLastName_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.FirstName = "Same";
        dto.LastName = "Same";

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "FirstName"));
    }

    // Employee must be at least 18 years old and not older than 70 years
    [TestMethod]
    public async Task Validate_TooYoungEmployee_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.Birthdate = DateTime.Now.AddYears(-17);

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Birthdate"));
    }

    [TestMethod]
    public async Task Validate_TooOldEmployee_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.Birthdate = DateTime.Now.AddYears(-71);

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Birthdate"));
    }

    // EmploymentDate cannot be earlier than 2000-01-01
    [TestMethod]
    public async Task Validate_EmploymentDateBefore2000_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.EmploymentDate = new DateTime(1999, 12, 31);

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "EmploymentDate"));
    }

    // EmploymentDate cannot be a future date
    [TestMethod]
    public async Task Validate_EmploymentDateInFuture_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.EmploymentDate = DateTime.Now.AddDays(1);

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "EmploymentDate"));
    }

    // Current salary must be non-negative
    [TestMethod]
    public async Task Validate_NegativeSalary_ShouldHaveValidationError()
    {
        var dto = CreateValidEmployeeDto();
        dto.CurrentSalary = -100;

        var result = await _validator.ValidateAsync(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "CurrentSalary"));
    }
}
