using VismaAPI.Application;
using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPIUnitTests.Services;

[TestClass]
public class EmployeeMapperTests
{
    [TestMethod]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var dto = new EmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            BossId = 1,
            HomeAddress = "Street 123",
            CurrentSalary = 2000m,
            Role = Role.Employee
        };

        var employee = dto.ToEntity();

        Assert.AreEqual(dto.FirstName, employee.FirstName);
        Assert.AreEqual(dto.LastName, employee.LastName);
        Assert.AreEqual(dto.Birthdate, employee.Birthdate);
        Assert.AreEqual(dto.EmploymentDate, employee.EmploymentDate);
        Assert.AreEqual(dto.BossId, employee.BossId);
        Assert.AreEqual(dto.HomeAddress, employee.HomeAddress);
        Assert.AreEqual(dto.CurrentSalary, employee.CurrentSalary);
        Assert.AreEqual(dto.Role, employee.Role);
    }

    [TestMethod]
    public void Map_ShouldUpdateEmployeeFieldsCorrectly()
    {
        var employee = new Employee
        {
            FirstName = "Old",
            LastName = "Name",
            Birthdate = DateTime.Now.AddYears(-40),
            EmploymentDate = DateTime.Now.AddYears(-10),
            BossId = null,
            HomeAddress = "Old Address",
            CurrentSalary = 1000m,
            Role = Role.Boss
        };

        var dto = new EmployeeDto
        {
            FirstName = "New",
            LastName = "Name",
            Birthdate = DateTime.Now.AddYears(-30),
            EmploymentDate = DateTime.Now.AddYears(-5),
            BossId = 2,
            HomeAddress = "New Address",
            CurrentSalary = 3000m,
            Role = Role.Employee
        };

        employee.Map(dto);

        Assert.AreEqual(dto.FirstName, employee.FirstName);
        Assert.AreEqual(dto.LastName, employee.LastName);
        Assert.AreEqual(dto.Birthdate, employee.Birthdate);
        Assert.AreEqual(dto.EmploymentDate, employee.EmploymentDate);
        Assert.AreEqual(dto.BossId, employee.BossId);
        Assert.AreEqual(dto.HomeAddress, employee.HomeAddress);
        Assert.AreEqual(dto.CurrentSalary, employee.CurrentSalary);
        Assert.AreEqual(dto.Role, employee.Role);
    }
}
