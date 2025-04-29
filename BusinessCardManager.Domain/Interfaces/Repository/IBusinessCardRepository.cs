using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Infrastructure.Interfaces;

namespace BusinessCardManager.Domain.Interfaces.Repository
{
    public interface IBusinessCardRepository : IBaseRepository<BusinessCard>
    {
        Task<BusinessCardDto> GetBusinessCardById(int id);
        Task<PagedResult<BusinessCardDto>> GetAllBusinessCards(BusinessCardFilterDto filter);
    }
}
