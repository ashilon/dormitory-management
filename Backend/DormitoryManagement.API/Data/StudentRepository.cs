using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using DormitoryManagement.API.Configuration;
using DormitoryManagement.API.Models;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Data;

public class StudentRepository : BaseRepository, IStudentRepository
{
    public StudentRepository(IOptions<DatabaseOptions> databaseOptions)
        : base(databaseOptions) { }

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Student>(
            "SELECT Id, Name, IdNumber, Age, EducationPlaceId, IsActive " +
            "FROM dbo.Student WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<int> UpsertAsync(StudentUpsertDto dto)
    {
        using var connection = CreateConnection();
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
