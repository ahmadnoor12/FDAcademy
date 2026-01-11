using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Enrollment
{
    public class CourseStudentsDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }

        public List<string> Students { get; set; }


    }
}
