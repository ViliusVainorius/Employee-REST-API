using VismaAPI.Domain;

namespace VismaAPI.Models;

public class EmployeeDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime Birthdate { get; set; }
    public required DateTime EmploymentDate { get; set; }
    public int? BossId { get; set; }
    public required string HomeAddress { get; set; }
    public required decimal CurrentSalary { get; set; }
    public required Role Role { get; set; }
}
