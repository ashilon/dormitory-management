using System.ComponentModel.DataAnnotations;

namespace DormitoryManagement.API.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;
}