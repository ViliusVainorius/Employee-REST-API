using Microsoft.AspNetCore.Mvc;
using VismaAPI.Application;
using VismaAPI.Domain;
using VismaAPI.Models;

namespace VismaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService service) : ControllerBase
{
    /// <summary>
    /// Getting a particular employee by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await service.GetByIdAsync(id);

        if (employee == null)
            return NotFound();

        return employee;
    }

    /// <summary>
    /// Search employees by name and birthdate interval
    /// </summary>
    /// <param name="name"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployees([FromQuery] SearchEmployeeDto searchEmployeeDto)
    {
        var employees = await service.SearchByNameAndBirthdateAsync(searchEmployeeDto);

        if (employees == null)
            return Ok(Enumerable.Empty<Employee>());

        return Ok(employees);
    }

    /// <summary>
    /// Getting all employees
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        var employees = await service.GetAllAsync();

        return Ok(employees);
    }

    /// <summary>
    /// Get employees by boss id
    /// </summary>
    /// <param name="bossId"></param>
    /// <returns></returns>
    [HttpGet("boss/{bossId:int}")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetByBossId(int bossId)
    {
        var employees = await service.GetByBossIdAsync(bossId);

        if (employees == null)
            return Ok(Enumerable.Empty<Employee>());

        return Ok(employees);
    }

    /// <summary>
    /// Get employee count and average salary for a role
    /// </summary>
    /// <param name="role">0 - CEO, 1 - Boss, 2 - Employee</param>
    /// <returns></returns>
    [HttpGet("role/{role}")]
    public async Task<ActionResult> GetCountAndAverageSalary(Role role)
    {
        var roleStatDto = await service.GetCountAndAverageSalaryByRoleAsync(role);

        return Ok(roleStatDto);
    }

    /// <summary>
    /// Adding new employee
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee([FromBody] EmployeeDto employee)
    {
        var createdEmployee = await service.CreateAsync(employee);

        return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Id }, createdEmployee);
    }

    /// <summary>
    /// Updating employee
    /// </summary>
    /// <param name="id"></param>
    /// <param name="employee"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto employeeDto)
    {
        var isUpdated = await service.UpdateAsync(id, employeeDto);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Updating just employee salary
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newSalary"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/salary")]
    public async Task<IActionResult> UpdateSalary(int id, [FromBody] UpdateSalaryDto salaryDto)
    {
        var isUpdated = await service.UpdateSalaryAsync(id, salaryDto);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deleting employee
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var isDeleted = await service.DeleteAsync(id);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }
}
