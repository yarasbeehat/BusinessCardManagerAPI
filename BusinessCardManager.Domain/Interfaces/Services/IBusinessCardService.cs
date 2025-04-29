using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Domain.Interfaces.Services
{
    public interface IBusinessCardService
    {        
        Task<PagedResult<BusinessCardDto>> GetAllBusinessCards(BusinessCardFilterDto filter);
        Task<BusinessCardDto> GetBusinessCardById(int id);
        Task<List<BusinessCardDto>> GetAllMyBusinessCards();
        Task<int> AddBusinessCard(CreateBusinessCardDto dto);        
        Task<bool> UpdateBusinessCard(int id, CreateBusinessCardDto dto);
        Task<bool> RemoveBusinessCard(int id);
        Task<int> AddBusinessCardFromXml(string xmlContent);
        Task<int> AddBusinessCardFromCsv(string csvContent);      
        Task<byte[]> ExportAllBusinessCardsToXml(BusinessCardFilterDto businessCardFilterDto);
        Task<byte[]> ExportAllBusinessCardsToCsv(BusinessCardFilterDto businessCardFilterDto);
    }
}
