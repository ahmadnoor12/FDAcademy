using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Course
{
    public class UserCoursesFilterDto : CourseFilterDto
    {
        public int StudentId { get; set; }
    }
}
