using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Data;

public class EducationPlaceRepository : IEducationPlaceRepository
{
    private readonly string _connectionString;

    public EducationPlaceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<IEnumerable<EducationPlaceSummaryDto>> GetSummariesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<EducationPlaceSummaryDto>(
            "dbo.GetEducationPlaceSummaries",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var result = await connection.ExecuteScalarAsync<int?>(
            "SELECT 1 FROM dbo.EducationPlace WHERE Id = @Id",
            new { Id = id });
        return result.HasValue;
    }
}
