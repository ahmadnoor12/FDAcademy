using Application.Dtos.Course;
using Application.RepositoriesInterface;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class CourseSerivce : ICourseServicecs
    {
        private readonly IGenericRepository<Course> _courseRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGenericRepository<Enrollment> _enrollment;
        public CourseSerivce(IGenericRepository<Course> courseRepo,
            IGenericRepository<Category> categoryRepo,
            IHttpContextAccessor contextAccessor,
            IGenericRepository<Enrollment> enrollment)
        {
            _courseRepo = courseRepo;
            _categoryRepo = categoryRepo;
            _contextAccessor = contextAccessor;
            _enrollment = enrollment;
        }

        public async Task CreateCourse(CreateCourseDto course)
        {
            var IsCorNamExist = await _courseRepo.GetAll()
                 .AnyAsync(c => c.Title.ToLower().Trim() == course.Title.ToLower().Trim());
            if (IsCorNamExist)
            {
                throw new Exception("Course name already exists.");

            }
            if (course.Price < 0)
            {
                throw new Exception("Price must be greater than to 0");
            }

            if (course.StartDate.Date < DateTime.UtcNow.Date)
            {
                throw new Exception("Start date nust be after today");
            }

            if (course.EndDate <= course.StartDate)
                throw new Exception("End date must be after StartDate");

            var cor = new Course
            {
                CategoryId = course.CategoryId,
                Title = course.Title,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Price = course.Price,
            };

            await _courseRepo.Insert(cor);
            await _courseRepo.SaveChanges();
        }

        public async Task UpdateCourse(int id, CourseUpdeteDto course)
        {
            var corobj = await _courseRepo.GetById(id);
            if (corobj == null)
            {
                throw new Exception("Course not found");
            }
            if (corobj.StartDate.Date < DateTime.UtcNow)
            {
                throw new Exception("Cannt update this course");
            }
            if (corobj.Price < 0)
            {
                throw new Exception("Price must be greater than to 0");
            }

            if (course.EndCourse <= course.StartCourse)
            {
                throw new Exception("EndDate must be after StartDate");
            }
            if (corobj.EndDate >= corobj.StartDate)
                throw new Exception("Cannot update course after it has started");

            corobj.StartDate = course.StartCourse;
            corobj.EndDate = course.EndCourse;
            corobj.Description = course.Description;
            corobj.Title = course.Title;
            corobj.Price = course.Price;

            _courseRepo.Update(corobj);
            await _courseRepo.SaveChanges();
        }
        public async Task DeleteCourse(int id)
        {
            var course = await _courseRepo.GetAll()
                 .Include(x => x.Enrollments)
                 .FirstOrDefaultAsync(x => x.Id == id);

            if (course != null)
            {
                if (!course.Enrollments.Any())
                {
                    await _courseRepo.Delete(course);
                    await _courseRepo.SaveChanges();
                }
                else
                {
                    throw new Exception("Cannot delete course already have a student.");
                }
            }
            else
            {
                throw new Exception("Course not found");
            }
        }
        public async Task<CourseListDto> GetCourseById(int id)
        {
            var course = await _courseRepo.GetById(id);
            if (course == null)
            {
                throw new Exception("Course not fount");
            }
            var courDetails = new CourseListDto
            {
                CourseId = course.Id,
                Title = course.Title,
                CategoryId = course.CategoryId,
                Description = course.Description,
                Price = course.Price,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
            };
            return courDetails;
        }

        public async Task<List<CoursesDataTable>> GetCoursesForAdmin(CourseFilterDto filter)
        {
            var courses = _courseRepo.GetAll()
                 .Include(c => c.Enrollments)
                 .Include(c => c.Category)
                 .Where(c => (!string.IsNullOrEmpty(filter.Title) ? c.Title.Trim().ToLower().Contains(filter.Title.Trim().ToLower()) : true) &&
                    (filter.CategoryId.HasValue ? c.CategoryId == filter.CategoryId.Value : true))
                 .Select(c => new CoursesDataTable
                 {
                     Id = c.Id,
                     Title = c.Title,
                     StartDate = c.StartDate,
                     EndDate = c.EndDate,
                     Category = c.Category.Name,
                     Price = c.Price,
                     StudentsCount = c.Enrollments.Count()
                 }).OrderByDescending(x => x.Id);

            return await courses.ToListAsync();
        }

        public async Task<List<CoursesDataTable>> GetMyCourses(CourseFilterDto filter)
        {
            var currentStudentId = _contextAccessor.HttpContext.User.FindAll("StudentId");

            var courses = _enrollment.GetAll()
                 .Include(c => c.Course).ThenInclude(c => c.Category)
                 .Include(c => c.Student)
                 .Where(c => c.StudentId == Convert.ToInt32(currentStudentId) &&
                 (!string.IsNullOrEmpty(filter.Title) ? c.Course.Title.Trim().ToLower().Contains(filter.Title.Trim().ToLower()) : true) &&
                    (filter.CategoryId.HasValue ? c.Course.CategoryId == filter.CategoryId.Value : true))
                 .Select(c => new CoursesDataTable
                 {
                     Id = c.CourseId,
                     Title = c.Course.Title,
                     StartDate = c.Course.StartDate,
                     EndDate = c.Course.EndDate,
                     Category = c.Course.Category.Name,
                     Price = c.Course.Price
                 }).OrderByDescending(x => x.Id);

            return await courses.ToListAsync();
        }

        public async Task<List<CoursesDataTable>> GetCoursesByStudentId(UserCoursesFilterDto filter)
        {
            var courses = _enrollment.GetAll()
                 .Include(c => c.Course).ThenInclude(c => c.Category)
                 .Include(c => c.Student)
                 .Where(c => c.StudentId == filter.StudentId &&
                 (!string.IsNullOrEmpty(filter.Title) ? c.Course.Title.Trim().ToLower().Contains(filter.Title.Trim().ToLower()) : true) &&
                    (filter.CategoryId.HasValue ? c.Course.CategoryId == filter.CategoryId.Value : true))
                 .Select(c => new CoursesDataTable
                 {
                     Id = c.CourseId,
                     Title = c.Course.Title,
                     StartDate = c.Course.StartDate,
                     EndDate = c.Course.EndDate,
                     Category = c.Course.Category.Name,
                     Price = c.Course.Price
                 }).OrderByDescending(x => x.Id);

            return await courses.ToListAsync();
        }

        public async Task<List<CoursesDataTable>> GetCourses(CourseFilterDto filter)
        {
            var currentStudentId = _contextAccessor.HttpContext.User.FindFirstValue("StudentId");
            int? studentId = null;
            if (!string.IsNullOrEmpty(currentStudentId))
            {
                studentId = Convert.ToInt32(currentStudentId);
            }

            var courses = _courseRepo.GetAll()
                 .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                 .Include(c => c.Category)
                 .Where(c => c.StartDate > DateTime.UtcNow &&
                 (studentId.HasValue ? !c.Enrollments.Any(x => x.StudentId == studentId.Value) : true) &&
                 (!string.IsNullOrEmpty(filter.Title) ? c.Title.Trim().ToLower().Contains(filter.Title.Trim().ToLower()) : true) &&
                    (filter.CategoryId.HasValue ? c.CategoryId == filter.CategoryId.Value : true))
                 .Select(c => new CoursesDataTable
                 {
                     Id = c.Id,
                     Title = c.Title,
                     StartDate = c.StartDate,
                     EndDate = c.EndDate,
                     Category = c.Category.Name,
                     Price = c.Price
                 }).OrderByDescending(x => x.Id);

            return await courses.ToListAsync();
        }

        public async Task<List<CategoryList>> GetCategories()
        {
            var categories = await _categoryRepo.GetAll()
                .Select(c => new CategoryList
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                }).ToListAsync();
            return categories;
        }
    }
}


