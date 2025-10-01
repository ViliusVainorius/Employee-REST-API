using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VismaAPI.Data;
using VismaAPI.Domain;

namespace VismaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(VismaAPIContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await context.Employees.ToListAsync();
    }
}
