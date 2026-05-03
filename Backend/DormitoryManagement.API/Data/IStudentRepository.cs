using DormitoryManagement.API.Models;
using DormitoryManagement.API.Models.DTOs;

namespace DormitoryManagement.API.Data;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<int>      UpsertAsync(StudentUpsertDto dto);
}
