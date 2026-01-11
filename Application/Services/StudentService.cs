using Application.Dtos.Auth;
using Application.Dtos.Student;
using Application.RepositoriesInterface;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _studentRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<Enrollment> _enrollmentRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StudentService(IGenericRepository<Student> studentRepo, IGenericRepository<User> userRepo, IGenericRepository<Role> roleRepo, IGenericRepository<Enrollment> enrollmentRepo, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepo = studentRepo;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _enrollmentRepo = enrollmentRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task StudentReg(StudentregistrationDto student)
        {
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            bool passwordValidate = Regex.IsMatch(student.Password, passwordPattern);
            if (!passwordValidate)
            {
                throw new Exception("Passowrd is weaks");
            }

            string emailPattern = @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[A-Za-z]{2,}$";
            bool emailValidate = Regex.IsMatch(student.Email, emailPattern);
            if (!emailValidate)
            {
                throw new Exception("Email is not valid");
            }
            string mobilePattern = @"^(?:\+?962|00962)?0?7[7-9]\d{7}$";
            bool mobileValidate = Regex.IsMatch(student.PhoneNumber, mobilePattern);
            if (!mobileValidate)
            {
                throw new Exception("phone is Wrong");
            }


            var studentRoleId = (await _roleRepo.GetAll()
              .FirstOrDefaultAsync(s => s.Code == RoleEnum.Student))?.Id;

            var userObj = new User();
            userObj.Name = student.Name;
            userObj.Email = student.Email;
            userObj.PhoneNumber = student.PhoneNumber;
            userObj.RoleId = studentRoleId.Value;

            var passwordHasher = new PasswordHasher<User>();
            userObj.Password = passwordHasher.HashPassword(userObj, student.Password);

            await _userRepo.Insert(userObj);
            await _userRepo.SaveChanges();

            await _studentRepo.Insert(new Student
            {
                UserId = userObj.Id,
                BirthDate = student.Birthdate,
                University = student.University
            });
            await _studentRepo.SaveChanges();
        }
        public async Task UpdateMyAccount(StudentUpdateDto student)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentUser = await _userRepo.GetById(Convert.ToInt32(currentUserId));
            if (studentUser == null)
            {
                throw new Exception("User not found");
            }

            var studentObj = await _studentRepo.GetAll().FirstOrDefaultAsync(x => x.UserId == Convert.ToInt32(currentUserId));
            if (studentObj == null)
            {
                throw new Exception("User not found");
            }

            studentUser.Name = student.FullName;
            studentUser.Email = student.Email;
            studentUser.PhoneNumber = student.PhoneNumber;

            studentObj.BirthDate = student.BirthDate;
            studentObj.University = student.University;

            _userRepo.Update(studentUser);
            _studentRepo.Update(studentObj);
            await _userRepo.SaveChanges();
        }


        public async Task<StudentListDto> GetCurrentStudent()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var student = await _studentRepo.GetAll()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == Convert.ToInt32(userId));

            return student == null ? null : new StudentListDto
            {
                StudentId = student.Id,
                BirthDate = student.BirthDate,
                University = student.University,
                User = new UserListDto
                {
                    UserId = student.UserId,
                    Name = student.User.Name,
                    Email = student.User.Email,
                    PhoneNumber = student.User.PhoneNumber,
                }
            };
        }

        public async Task<List<StudentListDto>> GetStudentList(StudentFilterDto filter)
        {
            var students = _studentRepo.GetAll()
                .Include(x => x.User)
                .Where(student => (filter.BirthDate.HasValue ? student.BirthDate.Date == filter.BirthDate.Value.Date : true) &&
                (!string.IsNullOrEmpty(filter.Name) ? student.User.Name.Trim().ToLower().Contains(filter.Name.Trim().ToLower()) : true) &&
                (!string.IsNullOrEmpty(filter.Email) ? student.User.Email.Trim().ToLower().Contains(filter.Email.Trim().ToLower()) : true) &&
                (!string.IsNullOrEmpty(filter.University) ? student.University.Trim().ToLower().Contains(filter.University.Trim().ToLower()) : true) &&
                (!string.IsNullOrEmpty(filter.PhoneNumber) ? student.User.PhoneNumber.Trim().Contains(filter.PhoneNumber.Trim()) : true)
                ).AsQueryable();

            var studentResult = await students.Select
              (student => new StudentListDto
              {
                  StudentId = student.Id,
                  BirthDate = student.BirthDate,
                  University = student.University,
                  User = new UserListDto
                  {
                      UserId = student.UserId,
                      Name = student.User.Name,
                      Email = student.User.Email,
                      PhoneNumber = student.User.PhoneNumber,
                  }
              }
              ).ToListAsync();

            return studentResult;
        }

        public async Task ChangePassword(int userId, string newPassword)
        {
            var user = await _userRepo.GetById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("user not found.");
            }
            else if (user.Role.Code == RoleEnum.Admin)
            {
                throw new Exception("Cannt change admin password.");
            }

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, newPassword);

            _userRepo.Update(user);
            await _userRepo.SaveChanges();
        }

        public async Task DeleteStudent(int Id)
        {
            var student = await _studentRepo.GetById(Id);
            if (student == null)
            {
                throw new Exception("Student not found");
            }

            var isEnrollmentExist = await _enrollmentRepo.GetAll()
                .AnyAsync(c => c.StudentId == Id);

            if (isEnrollmentExist)
            {
                throw new Exception("Cannt delete this student");
            }

            var user = await _userRepo.GetById(student.UserId);
            await _studentRepo.Delete(student);
            await _userRepo.Delete(user);
            await _studentRepo.SaveChanges();
        }
    }
}