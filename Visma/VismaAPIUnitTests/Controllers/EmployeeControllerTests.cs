using Microsoft.AspNetCore.Mvc;
using Moq;
using VismaAPI.Application;
using VismaAPI.Controllers;
using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPIUnitTests.Controllers;

[TestClass]
public class EmployeesControllerTests
{
    private Mock<IEmployeeService> _serviceMock = null!;
    private EmployeesController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceMock = new Mock<IEmployeeService>();
        _controller = new EmployeesController(_serviceMock.Object);
    }

    [TestMethod]
    public async Task GetEmployee_ShouldReturnEmployee_WhenExists()
    {
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street 123",
            CurrentSalary = 1000m,
            Role = Role.Employee
        };

        _serviceMock.Setup(s => s.GetByIdAsync(employee.Id)).ReturnsAsync(employee);

        var result = await _controller.GetEmployee(employee.Id);

        Assert.IsNotNull(result.Value);
        Assert.AreEqual(employee.Id, result.Value.Id);
        Assert.AreEqual(employee.FirstName, result.Value.FirstName);
        Assert.AreEqual(employee.LastName, result.Value.LastName);
        Assert.AreEqual(employee.Birthdate, result.Value.Birthdate);
        Assert.AreEqual(employee.EmploymentDate, result.Value.EmploymentDate);
        Assert.AreEqual(employee.HomeAddress, result.Value.HomeAddress);
        Assert.AreEqual(employee.CurrentSalary, result.Value.CurrentSalary);
        Assert.AreEqual(employee.Role, result.Value.Role);
    }

    [TestMethod]
    public async Task GetEmployee_ShouldReturnNotFound_WhenNotExists()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Employee?)null);

        var result = await _controller.GetEmployee(1);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetEmployees_ShouldReturnListOfEmployees()
    {
        var employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now.AddYears(-30),
                EmploymentDate = DateTime.Now.AddYears(-5),
                HomeAddress = "Street 123",
                CurrentSalary = 1000m,
                Role = Role.Employee
            }
        };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(employees);

        var result = await _controller.GetEmployees();

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        Assert.AreEqual(employees, ((OkObjectResult)result.Result).Value);
    }

    [TestMethod]
    public async Task CreateEmployee_ShouldReturnCreatedEmployee()
    {
        var employeeDto = new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street 123",
            CurrentSalary = 1000m,
            Role = Role.Employee
        };

        var createdEmployee = new Employee
        {
            Id = 1,
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Birthdate = employeeDto.Birthdate,
            EmploymentDate = employeeDto.EmploymentDate,
            HomeAddress = employeeDto.HomeAddress,
            CurrentSalary = employeeDto.CurrentSalary,
            Role = employeeDto.Role
        };

        _serviceMock.Setup(s => s.CreateAsync(employeeDto)).ReturnsAsync(createdEmployee);

        var result = await _controller.CreateEmployee(employeeDto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        Assert.AreEqual(createdEmployee, ((CreatedAtActionResult)result.Result).Value);
    }

    [TestMethod]
    public async Task UpdateEmployee_ShouldReturnNoContent_WhenSuccessful()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<EmployeeDto>())).ReturnsAsync(true);

        var dto = new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street 123",
            CurrentSalary = 1000m,
            Role = Role.Employee
        };

        var result = await _controller.UpdateEmployee(1, dto);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task UpdateEmployee_ShouldReturnNotFound_WhenFails()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<EmployeeDto>())).ReturnsAsync(false);

        var dto = new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            HomeAddress = "Street 123",
            CurrentSalary = 1000m,
            Role = Role.Employee
        };

        var result = await _controller.UpdateEmployee(1, dto);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task UpdateSalary_ShouldReturnNoContent_WhenSuccessful()
    {
        _serviceMock.Setup(s => s.UpdateSalaryAsync(It.IsAny<int>(), It.IsAny<UpdateSalaryDto>())).ReturnsAsync(true);

        var dto = new UpdateSalaryDto { NewSalary = 2000m };

        var result = await _controller.UpdateSalary(1, dto);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task UpdateSalary_ShouldReturnNotFound_WhenFails()
    {
        _serviceMock.Setup(s => s.UpdateSalaryAsync(It.IsAny<int>(), It.IsAny<UpdateSalaryDto>())).ReturnsAsync(false);

        var dto = new UpdateSalaryDto { NewSalary = 2000m };

        var result = await _controller.UpdateSalary(1, dto);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task DeleteEmployee_ShouldReturnNoContent_WhenSuccessful()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

        var result = await _controller.DeleteEmployee(1);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task DeleteEmployee_ShouldReturnNotFound_WhenFails()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<int>())).ReturnsAsync(false);

        var result = await _controller.DeleteEmployee(1);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}