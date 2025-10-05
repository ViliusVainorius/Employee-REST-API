using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VismaAPI.Application;
using VismaAPI.Data;
using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPIUnitTests.Services;

[TestClass]
public class EmployeeServiceTests
{
    private VismaAPIContext _context = null!;
    private Mock<IValidator<EmployeeDto>> _employeeValidatorMock = null!;
    private Mock<IValidator<UpdateSalaryDto>> _updateSalaryValidatorMock = null!;
    private EmployeeService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<VismaAPIContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new VismaAPIContext(options);

        _employeeValidatorMock = new Mock<IValidator<EmployeeDto>>();
        _updateSalaryValidatorMock = new Mock<IValidator<UpdateSalaryDto>>();

        _employeeValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<EmployeeDto>(), default))
            .ReturnsAsync(new ValidationResult());

        _updateSalaryValidatorMock
            .Setup(v => v.Validate(It.IsAny<UpdateSalaryDto>()))
            .Returns(new ValidationResult());

        _service = new EmployeeService(_context, _employeeValidatorMock.Object, _updateSalaryValidatorMock.Object);
    }

    private EmployeeDto CreateValidEmployeeDto()
    {
        return new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Some Street",
            CurrentSalary = 1000,
            Role = Role.Employee
        };
    }

    [TestMethod]
    public async Task CreateAsync_ShouldAddEmployeeSuccessfully()
    {
        var dto = CreateValidEmployeeDto();

        var created = await _service.CreateAsync(dto);

        Assert.IsNotNull(created);
        Assert.AreEqual(dto.FirstName, created.FirstName);

        var inDb = await _context.Employees.FirstOrDefaultAsync();
        Assert.IsNotNull(inDb);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnEmployee_WhenExists()
    {
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street",
            CurrentSalary = 1000,
            Role = Role.Employee
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(employee.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(employee.FirstName, result.FirstName);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateEmployeeSuccessfully()
    {
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street",
            CurrentSalary = 1000,
            Role = Role.Employee
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var dto = CreateValidEmployeeDto();
        dto.FirstName = "Updated";

        var isUpdated = await _service.UpdateAsync(employee.Id, dto);

        Assert.IsTrue(isUpdated);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveEmployeeFromDatabase()
    {
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street",
            CurrentSalary = 1000,
            Role = Role.Employee
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        await _service.DeleteAsync(employee.Id);

        var deleted = await _context.Employees.FindAsync(employee.Id);
        Assert.IsNull(deleted);
    }

    [TestMethod]
    public async Task UpdateSalaryAsync_ShouldUpdateSalarySuccessfully()
    {
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street",
            CurrentSalary = 1000,
            Role = Role.Employee
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var dto = new UpdateSalaryDto { NewSalary = 2000 };

        var isUpdated = await _service.UpdateSalaryAsync(employee.Id, dto);

        Assert.IsTrue(isUpdated);
    }
}
