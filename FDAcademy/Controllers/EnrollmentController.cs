using Application.Dtos.Enrollment;
using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FDAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [Authorize(Roles = FDAConst.STUDENT_ROLE)]
        [HttpPost("Enroll/{courseId}")]
        public async Task<ActionResult<EnrollmentDto>> Enroll(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var dto = await _enrollmentService.EnrollStudentAsync(userId, courseId);
            return Ok(dto);
        }
    }
}