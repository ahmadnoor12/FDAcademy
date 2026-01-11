using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Auth
{
    public class ChangePasswordDto
    {
        [Required]
        public int studentId { get; set; }
        public string NewPassword { get; set; }
    }
}