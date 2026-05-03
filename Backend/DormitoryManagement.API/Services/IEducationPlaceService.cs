using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Services;

public interface IEducationPlaceService
{
    Task<IEnumerable<EducationPlaceSummaryDto>> GetSummariesAsync();
}
