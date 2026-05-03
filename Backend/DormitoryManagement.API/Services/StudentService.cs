using DormitoryManagement.API.Data;
using DormitoryManagement.API.Exceptions;
using DormitoryManagement.API.Models;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository        _studentRepository;
    private readonly IEducationPlaceRepository _educationPlaceRepository;
    private readonly ILogger<StudentService>   _logger;

    public StudentService(
        IStudentRepository        studentRepository,
        IEducationPlaceRepository educationPlaceRepository,
        ILogger<StudentService>   logger)
    {
        _studentRepository        = studentRepository;
        _educationPlaceRepository = educationPlaceRepository;
        _logger                   = logger;
    }

    public async Task<(Student student, bool isNew)> UpsertAsync(StudentUpsertDto dto)
    {
        // Cross-entity validation: ensure the referenced dormitory exists
        if (!await _educationPlaceRepository.ExistsAsync(dto.EducationPlaceId))
            throw new NotFoundException(
                $"EducationPlace with Id '{dto.EducationPlaceId}' was not found.");

        bool isNew = !dto.Id.HasValue || dto.Id.Value == 0;

        if (!isNew)
        {
            // Ensure the student being updated actually exists
            var existing = await _studentRepository.GetByIdAsync(dto.Id!.Value);
            if (existing is null)
                throw new NotFoundException($"Student with Id '{dto.Id}' was not found.");
        }

        var studentId = await _studentRepository.UpsertAsync(dto);

        var student = await _studentRepository.GetByIdAsync(studentId)
            ?? throw new InvalidOperationException(
                $"Failed to retrieve student after upsert (Id={studentId}).");

        _logger.LogInformation(
            "{Operation} student Id={Id} Name={Name} EducationPlaceId={EducationPlaceId}",
            isNew ? "Created" : "Updated",
            studentId,
            dto.Name,
            dto.EducationPlaceId);

        return (student, isNew);
    }

    public async Task<Student> GetByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
            throw new NotFoundException($"Student with Id '{id}' was not found.");
        return student;
    }
}
