using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPI.Application;

public interface IEmployeeService
{
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> SearchByNameAndBirthdateAsync(SearchEmployeeDto dto);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetByBossIdAsync(int bossId);
    Task<RoleStatsDto> GetCountAndAverageSalaryByRoleAsync(Role role);
    Task<Employee> CreateAsync(EmployeeDto dto);
    Task<bool> UpdateAsync(int id, EmployeeDto dto);
    Task<bool> UpdateSalaryAsync(int id, UpdateSalaryDto salaryDto);
    Task<bool> DeleteAsync(int id);
}
