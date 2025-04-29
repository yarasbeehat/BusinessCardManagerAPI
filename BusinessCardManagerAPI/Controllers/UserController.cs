using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("/api/GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(UserFilterDto filter)
            => Ok(await _userService.GetAllUsers(filter));

        [HttpGet("/api/GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }


        [HttpPut("/api/UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            await _userService.UpdateUser(id, dto);
            return Ok();
        }

        [HttpDelete("/api/RemoveUser/{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            await _userService.RemoveUser(id);
            return Ok();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            //if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) ||  dto.RoleNames.Count == 0)
            //{
            //    return BadRequest("Email, password, and roles are required.");
            //}

            var id = await _userService.Register(dto);
            return Ok($"User with Id {id} was added successfully");
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            AuthenticationDto result = await _userService.Login(dto);
            if (result == null)
            {
                return Unauthorized("Username Or Password is incorrect");
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
