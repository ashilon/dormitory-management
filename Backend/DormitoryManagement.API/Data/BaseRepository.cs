using System.Data;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using DormitoryManagement.API.Configuration;

namespace DormitoryManagement.API.Data;

public abstract class BaseRepository
{
    private readonly string _connectionString;

    protected BaseRepository(IOptions<DatabaseOptions> databaseOptions)
    {
        _connectionString = databaseOptions.Value.ConnectionString;

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Database:ConnectionString is not configured.");
    }

    protected IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}