using DormitoryManagement.API.Models;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Services;

public interface IStudentService
{
    Task<(Student student, bool isNew)> UpsertAsync(StudentUpsertDto dto);
    Task<Student>                       GetByIdAsync(int id);
}
