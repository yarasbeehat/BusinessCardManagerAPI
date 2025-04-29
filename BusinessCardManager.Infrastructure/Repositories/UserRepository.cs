using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Domain.Interfaces.Repository;
using BusinessCardManager.Infrastructure.Context;
using BusinessCardManager.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly BusinessCardManagerContext _context;

        public UserRepository(BusinessCardManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<UserDto>> GetAllUsers(UserFilterDto filter)
        {
            
            Expression<Func<User, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.FirstName) || x.FirstName.ToLower().Contains(filter.FirstName.ToLower())) &&
                (string.IsNullOrEmpty(filter.LastName) || x.LastName.ToLower().Contains(filter.LastName.ToLower())) &&
                (string.IsNullOrEmpty(filter.PhoneNumber) || x.PhoneNumber.ToLower().Contains(filter.PhoneNumber.ToLower())) &&
                (string.IsNullOrEmpty(filter.Email) || x.Email.ToLower().Contains(filter.Email.ToLower()));

           
            var users = _context.Users
                .Where(predicate) 
                .OrderByDescending(x => x.CreatedOn)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email,                                        
                    CreatedBy = u.CreatedBy,
                    CreatedOn = u.CreatedOn,
                    BusinessCards = u.BusinessCards.Select(b => new BusinessCardDto
                    {
                        BusinessCardId = b.BusinessCardId,
                        Name = b.Name,
                        Gender = b.Gender,
                        DateOfBirth = b.DateOfBirth,
                        Email = b.Email,
                        Phone = b.Phone,
                        Address = b.Address,
                        PhotoBase64 = b.PhotoBase64,
                        UserId = b.UserId,
                        CreatedBy = b.CreatedBy,
                        CreatedOn = b.CreatedOn
                    }).ToList() 
                })
                .AsQueryable();
            
            return new PagedResult<UserDto>
            {
                Result = users.ApplyPaging(filter.Page, filter.PageSize).ToList(),
                TotalCount = await users.CountAsync()
            };
        }

        public async Task<User> GetUserWithDetails(int id)
        {
            return await _context.Users
                .Include(x => x.RoleUsers)
                .Include(x => x.BusinessCards)
                .FirstOrDefaultAsync(x => x.UserId == id && !x.IsDeleted);
        }

        public async Task<User> Login(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.FirstOrDefaultAsync(predicate);
        }
    }
}
