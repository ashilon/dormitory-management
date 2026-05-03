namespace DormitoryManagement.API.Models;

public class Student
{
    public int    Id               { get; set; }
    public string Name             { get; set; } = string.Empty;
    public string IdNumber         { get; set; } = string.Empty;
    public int    Age              { get; set; }
    public int    EducationPlaceId { get; set; }
    public bool   IsActive         { get; set; }
}
