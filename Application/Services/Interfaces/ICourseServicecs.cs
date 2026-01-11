using Application.Dtos.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{

    public interface ICourseServicecs
    {
        Task CreateCourse(CreateCourseDto course);
        Task UpdateCourse(int Id, CourseUpdeteDto course);
        Task DeleteCourse(int id);
        Task<List<CoursesDataTable>> GetCoursesForAdmin(CourseFilterDto filter);
        Task<CourseListDto> GetCourseById(int id);
        Task<List<CoursesDataTable>> GetMyCourses(CourseFilterDto filter);
        Task<List<CoursesDataTable>> GetCourses(CourseFilterDto filter);
        Task<List<CategoryList>> GetCategories();
        Task<List<CoursesDataTable>> GetCoursesByStudentId(UserCoursesFilterDto filter);
    }
}
