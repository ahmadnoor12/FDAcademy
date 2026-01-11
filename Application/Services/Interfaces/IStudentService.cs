using Application.Dtos.Auth;
using Application.Dtos.Course;
using Application.Dtos.Student;

namespace Application.Services.Interfaces
{
    public interface IStudentService
    {
        Task StudentReg(StudentregistrationDto student);
        Task UpdateMyAccount(StudentUpdateDto student);
        Task<StudentListDto> GetCurrentStudent();
        Task<List<StudentListDto>> GetStudentList(StudentFilterDto filter);
        Task DeleteStudent(int Id);
        Task ChangePassword(int userId, string newPassword);

    }
}