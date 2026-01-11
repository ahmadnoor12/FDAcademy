using Application.Dtos.Enrollment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> EnrollStudentAsync(int StudentId, int CourseId);
    }
}
