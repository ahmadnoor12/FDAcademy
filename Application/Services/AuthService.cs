using Application.Dtos.Auth;
using Application.RepositoriesInterface;
using Application.Services.Interfaces;
using Domain;
using Domain.Entities;
using Domain.Entities.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IConfiguration _config;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<Student> _studentRepo;


        public AuthService(IGenericRepository<User> userRepo,
            IConfiguration config,
            IGenericRepository<RefreshToken> refreshTokenRepo,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Student> studentRepo)

        {
            _userRepo = userRepo;
            _config = config;
            _refreshTokenRepo = refreshTokenRepo;
            _httpContextAccessor = httpContextAccessor;
            _studentRepo = studentRepo;
        }


        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto input)
        {
            var user = await _userRepo.GetAll()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == input.Username.Trim().ToLower() || u.PhoneNumber.Trim() == input.Username.Trim());

            if (user == null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<User>();
            var passowrdResult = passwordHasher.VerifyHashedPassword(user, user.Password, input.Password);

            if (passowrdResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            await _refreshTokenRepo.Insert(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = new LoginRoleDto
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name,
                    Code = user.Role.Code,
                }
            };
        }

        public string GenerateAccessToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
            };
            var student = _studentRepo.GetAll().FirstOrDefault(s => s.UserId == user.Id);

            if (student != null)
            {
                claims.Add(new Claim("StudentId", student.Id.ToString()));
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);

        }

        public string GenerateRefreshToken()
        {
            var random = new byte[64];
            RandomNumberGenerator.Fill(random);
            return Convert.ToBase64String(random);
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Convert.ToInt32(userIdClaim);

            var storedToken = _refreshTokenRepo.GetAll()
                .FirstOrDefault(rt => rt.UserId == userId && rt.Token == refreshToken && rt.Expires > DateTime.UtcNow);
            if (storedToken == null)
            {
                throw new SecurityTokenException("Invalid refresh token.");
            }
            var user = await _userRepo.GetById(storedToken.UserId);
            return GenerateAccessToken(user);
        }

        public async Task ResetPassword(ResetPasswordDto input)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Convert.ToInt32(userIdClaim);
            var user = await _userRepo.GetById(userId);

            var passwordHasher = new PasswordHasher<User>();
            var passowrdResult = passwordHasher.VerifyHashedPassword(user, user.Password, input.OldPassword);

            if (passowrdResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Old password is incorrect.");
            }

            user.Password = passwordHasher.HashPassword(user, input.NewPassword);
            _userRepo.Update(user);
            await _userRepo.SaveChanges();
        }

    }
}