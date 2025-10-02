using Microsoft.EntityFrameworkCore;
using VismaAPI.Data;
using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPI.Application;

public class EmployeeService(VismaAPIContext context) : IEmployeeService
{
    public async Task<Employee?> GetByIdAsync(int id) =>
        await context.Employees.FindAsync(id);

    public async Task<IEnumerable<Employee>> SearchByNameAndBirthdateAsync(SearchEmployeeDto dto)
    {
        var query = context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Name))
            query = query.Where(e => e.FirstName.Contains(dto.Name) || e.LastName.Contains(dto.Name));

        if (dto.From.HasValue)
            query = query.Where(e => e.Birthdate >= dto.From.Value);

        if (dto.To.HasValue)
            query = query.Where(e => e.Birthdate <= dto.To.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetAllAsync() =>
        await context.Employees.ToListAsync();

    public async Task<IEnumerable<Employee>> GetByBossIdAsync(int bossId)
    {
        return await context.Employees
            .Where(e => e.BossId == bossId)
            .ToListAsync();
    }

    public async Task<RoleStatsDto> GetCountAndAverageSalaryByRoleAsync(Role role)
    {
        var employees = context.Employees.Where(e => e.Role == role);

        var count = await employees.CountAsync();

        var averageSalary = count > 0 ? await employees.AverageAsync(e => e.CurrentSalary) : 0m;

        return new RoleStatsDto
        {
            Count = count,
            AverageSalary = averageSalary
        };
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        context.Employees.Add(employee);

        await context.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee> CreateAsync(EmployeeDto dto)
    {
        var employee = dto.ToEntity();

        context.Employees.Add(employee);

        await context.SaveChangesAsync();

        return employee;
    }

    public async Task<bool> UpdateAsync(int id, EmployeeDto dto)
    {
        var employee = await context.Employees.FindAsync(id);

        if (employee == null)
            return false;

        employee.Map(dto);

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateSalaryAsync(int id, UpdateSalaryDto salaryDto)
    {
        var employee = await context.Employees.FindAsync(id);

        if (employee == null)
            return false;

        employee.CurrentSalary = salaryDto.NewSalary;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);

        if (employee == null)
            return false;

        context.Employees.Remove(employee);

        await context.SaveChangesAsync();

        return true;
    }
}
