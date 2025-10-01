using System.ComponentModel.DataAnnotations;

namespace VismaAPI.Domain;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]// pakeisti i fluentvalidation
    public required string FirstName { get; set; }
    [Required]
    [MaxLength(50)]// pakeisti i fluentvalidation
    public required string LastName { get; set; }
    [Required]
    public required DateTime Birthdate { get; set; }
    [Required]
    public required DateTime EmploymentDate { get; set; }

    public int? BossId { get; set; }
    public Employee? Boss { get; set; }

    [Required]
    public required string HomeAddress { get; set; }
    [Required]
    public required decimal CurrentSalary { get; set; }
    [Required]
    public required string Role { get; set; }
}
