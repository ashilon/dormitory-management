using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DormitoryManagement.API.Models;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Data;

public class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Student>(
            "SELECT Id, Name, IdNumber, Age, EducationPlaceId, IsActive " +
            "FROM dbo.Student WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<int> UpsertAsync(StudentUpsertDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "dbo.UpsertStudent",
            new
            {
                dto.Id,
                dto.Name,
                dto.IdNumber,
                dto.Age,
                dto.EducationPlaceId,
                dto.IsActive
            },
            commandType: CommandType.StoredProcedure);
    }
}
