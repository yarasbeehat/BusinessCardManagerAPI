using AutoMapper;
using System.Security.Claims;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Interfaces.Services;

namespace BusinessCardManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
                
        public async Task<PagedResult<UserDto>> GetAllUsers(UserFilterDto filter)
            => await _unitOfWork.UserRepository.GetAllUsers(filter);

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserWithDetails(id);

            if (user == null)
                throw new Exception($"User with id {id} not found");

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUser(int id, UpdateUserDto dto)
        {
            var data = await _unitOfWork.UserRepository.GetFirstOrDefaultWithInclude(
                x => x.UserId == id && !x.IsDeleted);
            if (data == null)
                throw new Exception($"User with id {id} not found");

            var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultWithInclude(
                u => u.Email == dto.Email);
            if (existingUser != null)
                throw new Exception("The email is already in use");

            var user = _mapper.Map(dto, data);
            user.UpdatedOn = DateTime.Now;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                userId = "Anonymous";
            user.UpdatedBy = userId;
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.Commit();
        }

        public async Task RemoveUser(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserWithDetails(id);

            if (user == null)
                throw new Exception($"User with id {id} not found");

            await RemoveRoleUsers(user);
            await RemoveBusinessCards(user);

            user.IsDeleted = true;
            user.UpdatedOn = DateTime.Now;

            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                currentUserId = "Anonymous";
            user.UpdatedBy = currentUserId;

            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.Commit();
        }

        private async Task RemoveRoleUsers(User user)
        {
            foreach (var roleUser in user.RoleUsers.ToList())
            {
                roleUser.IsDeleted = true;
                roleUser.UpdatedOn = DateTime.Now;
                var roleUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(roleUserId))
                    roleUserId = "Anonymous";
                roleUser.UpdatedBy = roleUserId;
                await _unitOfWork.RoleUserRepository.Update(roleUser);
            }
        }

        private async Task RemoveBusinessCards(User user)
        {
            foreach (var taskAssignee in user.BusinessCards.ToList())
            {
                taskAssignee.IsDeleted = true;
                taskAssignee.UpdatedOn = DateTime.Now;
                var taskAssigneeUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(taskAssigneeUserId))
                    taskAssigneeUserId = "Anonymous";
                taskAssignee.UpdatedBy = taskAssigneeUserId;
                await _unitOfWork.BusinessCardRepository.Update(taskAssignee);
            }
        }


        public async Task<int> Register(CreateUserDto dto)
        {
            var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultWithInclude(
                u => u.Email == dto.Email);
            if (existingUser != null)
                throw new Exception("The email is already in use");

            using var hmac = new HMACSHA512();

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password cannot be empty.");

            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            byte[] passwordSalt = hmac.Key;

            if (passwordHash == null || passwordHash.Length == 0 || passwordSalt == null || passwordSalt.Length == 0)
                throw new Exception("Password hash or salt cannot be null or empty.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedOn = DateTime.Now;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                userId = "Anonymous";
            user.CreatedBy = userId;

            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.Commit();

            //if (user.UserId == 0)                
            //    throw new Exception("User was not saved correctly.");

            var roles = await _unitOfWork.RoleRepository.GetAllWithIncludes();
            if (roles == null || !roles.Any())
                throw new Exception("No roles found in the system.");

            var userRoles = roles.Select(role => new RoleUser
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            }).ToList();

            await _unitOfWork.RoleUserRepository.AddRange(userRoles);
            var rolesList = userRoles.Select(ur => ur.Role).ToList();

            var token = GenerateToken(user.UserId, rolesList);

            return user.UserId;
        }

        public async Task<AuthenticationDto> Login(LoginDto dto)
        {
            var user = await _unitOfWork.UserRepository.Login(a => a.Email.ToLower() == dto.Email.ToLower());
            if (user is null)
                throw new UnauthorizedAccessException("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    throw new UnauthorizedAccessException("Invalid password");
            }

            var roles = await _unitOfWork.RoleRepository.GetAllWithIncludes(
                role => role.RoleUsers.Any(ur => ur.UserId == user.UserId));

            var rolesList = roles.ToList();
            var token = GenerateToken(user.UserId, rolesList);
            AuthenticationDto authenticationDTO = new AuthenticationDto()
            {
                AccessToken = GenerateToken(user.UserId, rolesList),
                ExpiresAt = DateTime.Now.AddHours(1),
                UserId = user.UserId
            };
            return authenticationDTO;
        }

        private string GenerateToken(int userId, List<Role> rolesList)
        {

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };
            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
