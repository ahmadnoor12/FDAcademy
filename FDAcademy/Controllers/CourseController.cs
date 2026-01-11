using Application.Dtos.Course;
using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FDAcademy.Controllers
{
    [Authorize(Roles = FDAConst.ADMIN_ROLE)]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly ICourseServicecs _courseService;

        public CourseController(ICourseServicecs courseService)
        {
            _courseService = courseService;
        }
        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto course)
        {
            await _courseService.CreateCourse(course);
            return Ok("Course created successfully");
        }
        [HttpPost("UpdateCourse/{Id}")]
        public async Task<IActionResult> UpdateCourse(int Id, [FromBody] CourseUpdeteDto course)
        {
            await _courseService.UpdateCourse(Id, course);
            return Ok("Course updated successfully");
        }
        [HttpDelete("DeleteCourse/{Id}")]
        public async Task<IActionResult> DeleteCourse(int Id)
        {
            await _courseService.DeleteCourse(Id);
            return Ok("Course deleted successfully");
        }
        [Authorize(Roles = $"{FDAConst.ADMIN_ROLE}")]
        [HttpPost("GetCoursesForAdmin")]
        public async Task<ActionResult<List<CoursesDataTable>>> GetCoursesForAdmin([FromBody] CourseFilterDto filter)
        {
            var courses = await _courseService.GetCoursesForAdmin(filter);
            return Ok(courses);
        }
        [Authorize(Roles = $"{FDAConst.STUDENT_ROLE}")]
        [HttpPost("GetMyCourses")]
        public async Task<ActionResult<List<CoursesDataTable>>> GetMyCourses([FromBody] CourseFilterDto filter)
        {
            var courses = await _courseService.GetMyCourses(filter);
            return Ok(courses);
        }
        [Authorize(Roles = $"{FDAConst.ADMIN_ROLE}")]
        [HttpPost("GetCoursesByStudentId")]
        public async Task<ActionResult<List<CoursesDataTable>>> GetCoursesByStudentId([FromBody] UserCoursesFilterDto filter)
        {
            var courses = await _courseService.GetCoursesByStudentId(filter);
            return Ok(courses);
        }
        [AllowAnonymous]
        [HttpPost("GetCourses")]
        public async Task<ActionResult<List<CoursesDataTable>>> GetCourses([FromBody] CourseFilterDto filter)
        {
            var courses = await _courseService.GetCourses(filter);
            return Ok(courses);
        }

        [AllowAnonymous]
        [HttpGet("GetCategories")]
        public async Task<ActionResult<List<CategoryList>>> GetCategories()
        {
            var courses = await _courseService.GetCategories();
            return Ok(courses);
        }

        [Authorize(Roles = $"{FDAConst.ADMIN_ROLE}")]
        [HttpGet("GetCoursrById/{Id}")]
        public async Task<IActionResult> GetCoursrById(int Id)
        {
            var course = await _courseService.GetCourseById(Id);
            return Ok(course);
        }
    }
}
