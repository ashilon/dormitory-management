using Microsoft.AspNetCore.Mvc;
using DormitoryManagement.API.Models;
using DormitoryManagement.API.Models.DTOs;
using DormitoryManagement.API.Services;

namespace DormitoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService             _service;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IStudentService             service,
        ILogger<StudentsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>
    /// Upsert a student.
    /// - Id is null or 0  → INSERT (returns 201 Created)
    /// - Id is positive   → UPDATE (returns 200 OK)
    /// Model validation (data annotations) runs automatically before this action.
    /// Additional business validation (valid EducationPlaceId, student existence) runs in the service.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Student), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upsert([FromBody] StudentUpsertDto dto)
    {
        _logger.LogInformation(
            "POST /api/students Id={Id} Name={Name}", dto.Id, dto.Name);

        var (student, isNew) = await _service.UpsertAsync(dto);

        return isNew
            ? CreatedAtAction(nameof(GetById), new { id = student.Id }, student)
            : Ok(student);
    }

    /// <summary>Gets a single student by Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetByIdAsync(id);
        return Ok(student);
    }
}
