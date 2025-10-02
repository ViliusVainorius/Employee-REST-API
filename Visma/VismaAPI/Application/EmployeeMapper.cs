using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPI.Application;

public static class EmployeeMapper
{
    public static void Map(this Employee employee, EmployeeDto dto)
    {
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Birthdate = dto.Birthdate;
        employee.EmploymentDate = dto.EmploymentDate;
        employee.BossId = dto.BossId;
        employee.HomeAddress = dto.HomeAddress;
        employee.CurrentSalary = dto.CurrentSalary;
        employee.Role = dto.Role;
    }

    public static Employee ToEntity(this EmployeeDto dto)
    {
        return new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Birthdate = dto.Birthdate,
            EmploymentDate = dto.EmploymentDate,
            BossId = dto.BossId,
            HomeAddress = dto.HomeAddress,
            CurrentSalary = dto.CurrentSalary,
            Role = dto.Role
        };
    }
}
