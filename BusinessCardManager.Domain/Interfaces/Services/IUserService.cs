using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllUsers(UserFilterDto filter);
        Task<UserDto> GetUserById(int id);
        Task UpdateUser(int id, UpdateUserDto dto);
        Task RemoveUser(int id);
        Task<int> Register(CreateUserDto dto);
        Task<AuthenticationDto> Login(LoginDto dto);
    }
}
