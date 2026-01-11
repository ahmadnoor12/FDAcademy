using Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Course
{
    public class CategoryList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryEnum Code { get; set; }
    }
}
