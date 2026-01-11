using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Course
{
    public class CourseFilterDto
    {
        public string? Title { get; set; }
        public int? CategoryId { get; set; }
    }
}
