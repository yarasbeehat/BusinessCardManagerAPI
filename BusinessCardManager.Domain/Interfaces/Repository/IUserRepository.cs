using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Infrastructure.Interfaces;

namespace BusinessCardManager.Domain.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<PagedResult<UserDto>> GetAllUsers(UserFilterDto filter);
        Task<User> GetUserWithDetails(int id);

        Task<User> Login(Expression<Func<User, bool>> predicate);        
    }
}
