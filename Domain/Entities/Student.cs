using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public DateTime BirthDate { get; set; }
        public string University { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
