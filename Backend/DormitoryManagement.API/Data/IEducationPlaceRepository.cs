using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Data;

public interface IEducationPlaceRepository
{
    Task<IEnumerable<EducationPlaceSummaryDto>> GetSummariesAsync();
    Task<bool> ExistsAsync(int id);
}
