using Application.Dtos.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAdmin_Service
    {
        Task<StudentListDto> UpdateStudent(StudentUpdateDto student);
        Task DeleteStudent(int id);
        IQueryable<StudentListDto> GetAll();
        Task<StudentListDto> GetStudentById(int id);
    }
}
