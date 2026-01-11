using Application.Dtos.Enrollment;
using Application.RepositoriesInterface;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGenericRepository<Enrollment> _enrollmentRepo;
        private readonly IGenericRepository<Student> _studentRepo;
        private readonly IGenericRepository<Course> _courseRepo;

        public EnrollmentService(
            IGenericRepository<Enrollment> enrollmentRepo,
            IGenericRepository<Student> studentRepo,
            IGenericRepository<Course> courseRepo)
        {
            _enrollmentRepo = enrollmentRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
        }
        public async Task<EnrollmentDto> EnrollStudentAsync(int userId, int courseId)
        {
            var student = await _studentRepo.GetAll().Include(x => x.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                throw new Exception("Student not found.");

            var course = await _courseRepo.GetById(courseId);
            if (course == null)
                throw new Exception("Course not found.");

            var existingEnrollment = await _enrollmentRepo.GetAll()
                .AnyAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

            if (!existingEnrollment)
            {
                var enrollment = new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = courseId,
                    EnrollmentDate = DateTime.UtcNow
                };

                await _enrollmentRepo.Insert(enrollment);
                await _enrollmentRepo.SaveChanges();
            }
            else
            {
                throw new Exception("Student is already enrolled in this course.");
            }

            return new EnrollmentDto
            {
                StudentId = student.Id,
                StudentName = student.User.Name,
                CourseId = courseId,
                CourseTitle = course.Title,
                EnrollmentDate = DateTime.UtcNow
            };
        }
    }
}