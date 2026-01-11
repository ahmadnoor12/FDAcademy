using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Course
{
    public class CourseUpdeteDto
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Price { get; set; }
        public DateTime StartCourse { get; set; }
        public DateTime EndCourse { get; set; }
        public int CategoryId { get; set; }
    }
}
