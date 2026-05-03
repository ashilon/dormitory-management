using DormitoryManagement.API.Data;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Services;

public class EducationPlaceService : IEducationPlaceService
{
    private readonly IEducationPlaceRepository _repository;

    public EducationPlaceService(IEducationPlaceRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<EducationPlaceSummaryDto>> GetSummariesAsync()
        => _repository.GetSummariesAsync();
}
