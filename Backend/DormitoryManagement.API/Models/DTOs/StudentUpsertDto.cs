using System.ComponentModel.DataAnnotations;

namespace DormitoryManagement.API.Models.DTOs;

public class StudentUpsertDto
{
    /// <summary>Null or 0 → INSERT; positive value → UPDATE the existing record.</summary>
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "ID Number is required.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "ID Number must be exactly 9 digits.")]
    public string IdNumber { get; set; } = string.Empty;

    [Range(10, 120, ErrorMessage = "Age must be between 10 and 120.")]
    public int Age { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A valid EducationPlace Id is required.")]
    public int EducationPlaceId { get; set; }

    public bool IsActive { get; set; } = true;
}
