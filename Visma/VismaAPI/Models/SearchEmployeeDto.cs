namespace VismaAPI.Models;

public class SearchEmployeeDto
{
    public required string Name { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}
