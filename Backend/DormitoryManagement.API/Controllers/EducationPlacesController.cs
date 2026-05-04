using Microsoft.AspNetCore.Mvc;
using DormitoryManagement.API.Models.DTOs;
using DormitoryManagement.API.Services;

namespace DormitoryManagement.API.Controllers;

[ApiController]
[Route("api/education-places")]
public class EducationPlacesController : ControllerBase
{
    private readonly IEducationPlaceService             _service;
    private readonly ILogger<EducationPlacesController> _logger;

    public EducationPlacesController(
        IEducationPlaceService             service,
        ILogger<EducationPlacesController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>
    /// Returns all education places (dormitories) with the count of active students
    /// and their average age. Data is sourced from the GetEducationPlaceSummaries SP.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EducationPlaceSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummaries()
    {
        _logger.LogInformation("GET /api/education-places");
        var result = await _service.GetSummariesAsync();
        return Ok(result);
    }
}
