using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using DormitoryManagement.API.Configuration;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Data;

public class EducationPlaceRepository : BaseRepository, IEducationPlaceRepository
{
    public EducationPlaceRepository(IOptions<DatabaseOptions> databaseOptions)
        : base(databaseOptions) { }

    public async Task<IEnumerable<EducationPlaceSummaryDto>> GetSummariesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<EducationPlaceSummaryDto>(
            "dbo.GetEducationPlaceSummaries",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using var connection = CreateConnection();
        var result = await connection.ExecuteScalarAsync<int?>(
            "SELECT 1 FROM dbo.EducationPlace WHERE Id = @Id",
            new { Id = id });
        return result.HasValue;
    }
}
