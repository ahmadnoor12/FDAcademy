using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Enrollment
{
    public class StudentCoursesDto
    {
        public int? StudentId { get; set; }
        public string StudentName { get; set; }

        public List<string> Courses { get; set; }

    }
}
