namespace DormitoryManagement.API.Models.DTOs;

public class EducationPlaceSummaryDto
{
    public int     Id                 { get; set; }
    public string  Name               { get; set; } = string.Empty;
    public string  City               { get; set; } = string.Empty;
    public int     ActiveStudentCount { get; set; }
    public decimal AverageAge         { get; set; }
}
