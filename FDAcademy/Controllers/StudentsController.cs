using Application.Dtos.Auth;
using Application.Dtos.Student;
using Application.Services;
using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FDAcademy.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> StudentReg([FromBody] StudentregistrationDto student)
        {
            await _studentService.StudentReg(student);
            return Ok("Student registered successfully");
        }
        [Authorize(Roles = FDAConst.STUDENT_ROLE)]
        [HttpPost("UpdateMyAccount")]
        public async Task<IActionResult> UpdateMyAccount([FromBody] StudentUpdateDto student)
        {
            await _studentService.UpdateMyAccount(student);
            return Ok("Student updated successfully");
        }
        [Authorize(Roles = FDAConst.STUDENT_ROLE)]
        [HttpGet("GetCurrentStudent")]
        public async Task<IActionResult> GetCurrentStudent()
        {
            var student = await _studentService.GetCurrentStudent();
            if (student == null) return NotFound("Student not found");
            return Ok(student);

        }
        [Authorize(Roles = FDAConst.ADMIN_ROLE)]
        [HttpPost("GetStudentList")]
        public async Task<ActionResult<List<StudentListDto>>> GetList([FromBody] StudentFilterDto filter)
        {
            var students = await _studentService.GetStudentList(filter);
            return Ok(students);
        }
        [Authorize(Roles = FDAConst.ADMIN_ROLE)]
        [HttpPost("ChangePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto input)
        {
            await _studentService.ChangePassword(userId, input.NewPassword);
            return Ok("Password changed successfully.");

        }

        [Authorize(Roles = FDAConst.ADMIN_ROLE)]
        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudent(id);
            return Ok("Student deleted successfully");
        }
    }
}
