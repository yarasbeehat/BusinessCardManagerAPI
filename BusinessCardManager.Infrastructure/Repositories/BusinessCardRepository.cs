using System.Linq.Expressions;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Domain.Interfaces.Repository;
using BusinessCardManager.Infrastructure.Context;
using BusinessCardManager.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;


namespace BusinessCardManager.Infrastructure.Repositories
{
    public class BusinessCardRepository : BaseRepository<BusinessCard>, IBusinessCardRepository
    {
        private readonly BusinessCardManagerContext _context;

        public BusinessCardRepository(BusinessCardManagerContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BusinessCardDto> GetBusinessCardById(int id)
        {
            var businessCardDto = await _context.BusinessCards
        .Where(x => x.BusinessCardId == id && !x.IsDeleted)
        .Join(
            _context.Users, 
            card => card.UserId, 
            user => user.UserId, 
            (card, user) => new BusinessCardDto
            {
                BusinessCardId = card.BusinessCardId,
                Name = card.Name,
                Gender = card.Gender,
                DateOfBirth = card.DateOfBirth,
                Email = card.Email,
                Phone = card.Phone,
                Address = card.Address,
                PhotoBase64 = card.PhotoBase64,
                UserId = card.UserId,
                UserName = user.FirstName + " " + user.LastName, 
                CreatedBy = card.CreatedBy,
                CreatedOn = card.CreatedOn
            })
        .FirstOrDefaultAsync(); 

            return businessCardDto;
        }

        public async Task<PagedResult<BusinessCardDto>> GetAllBusinessCards(BusinessCardFilterDto filter)
        {
            Expression<Func<BusinessCard, bool>> predicate = x =>
            !x.IsDeleted &&  
            (!filter.BusinessCardId.HasValue || x.BusinessCardId == filter.BusinessCardId) &&
            (!filter.UserId.HasValue || x.UserId == filter.UserId) &&
            (string.IsNullOrEmpty(filter.Name) || x.Name.ToLower().Contains(filter.Name.ToLower())) &&
            (string.IsNullOrEmpty(filter.Gender) || x.Gender.ToLower() == filter.Gender.ToLower()) &&
            (!filter.DateOfBirth.HasValue || x.DateOfBirth == filter.DateOfBirth) &&
            (string.IsNullOrEmpty(filter.Email) || x.Email.ToLower().Contains(filter.Email.ToLower())) &&
            (string.IsNullOrEmpty(filter.Phone) || x.Phone.ToLower().Contains(filter.Phone.ToLower()));
            
            var businessCards = _context.BusinessCards
                .Where(predicate)
                .Include(x => x.User) 
                .OrderByDescending(x => x.CreatedOn)
                .Select(card => new BusinessCardDto
                {
                    BusinessCardId = card.BusinessCardId,
                    Name = card.Name,
                    Gender = card.Gender,
                    DateOfBirth = card.DateOfBirth,
                    Email = card.Email,
                    Phone = card.Phone,
                    Address = card.Address,
                    PhotoBase64 = card.PhotoBase64,
                    UserId = card.UserId,
                    UserName = card.User.FirstName + " " + card.User.LastName,
                    CreatedBy = card.CreatedBy,
                    CreatedOn = card.CreatedOn
                });

            return new PagedResult<BusinessCardDto>
            {
                Result = businessCards.ApplyPaging(filter.PageIndex, filter.PageSize).ToList(),
                TotalCount = await businessCards.CountAsync()
            };
        
        }
    }
}
