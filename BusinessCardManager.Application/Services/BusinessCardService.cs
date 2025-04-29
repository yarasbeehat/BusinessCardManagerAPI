using System.Globalization;
using System.Security.Claims;
using System.Xml.Serialization;
using AutoMapper;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Domain.Interfaces.Services;
using BusinessCardManager.Infrastructure.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;

namespace BusinessCardManager.Application.Services
{
    public class BusinessCardService : IBusinessCardService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BusinessCardService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }       

        public async Task<PagedResult<BusinessCardDto>> GetAllBusinessCards(BusinessCardFilterDto filter)
            => await _unitOfWork.BusinessCardRepository.GetAllBusinessCards(filter);

        public async Task<BusinessCardDto> GetBusinessCardById(int id)
        {
            var businessCard = await _unitOfWork.BusinessCardRepository.GetBusinessCardById(id);

            if (businessCard == null)
                throw new Exception($"BusinessCard with id {id} not found");

            return _mapper.Map<BusinessCardDto>(businessCard);
        }

        public async Task<List<BusinessCardDto>> GetAllMyBusinessCards()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                throw new InvalidOperationException("Invalid user ID");

            var businessCards = await _unitOfWork.BusinessCardRepository.GetAllWithIncludes(
                x => !x.IsDeleted && x.UserId == userId);
            return _mapper.Map<List<BusinessCardDto>>(businessCards);
        }

        public async Task<int> AddBusinessCard(CreateBusinessCardDto dto)
        {
            var businessCard = _mapper.Map<BusinessCard>(dto);
            businessCard.CreatedOn = DateTime.Now;            
        
            var businessCardId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(businessCardId))
                businessCardId = "Anonymous";
            businessCard.CreatedBy = businessCardId;

            if (!string.IsNullOrEmpty(dto.PhotoBase64))
            {
                businessCard.PhotoBase64 = dto.PhotoBase64;  
            }

            await _unitOfWork.BusinessCardRepository.Add(businessCard);
            await _unitOfWork.Commit();
            return businessCard.BusinessCardId;
        }

        public async Task<bool> UpdateBusinessCard(int id, CreateBusinessCardDto dto)
        {
            var data = await _unitOfWork.BusinessCardRepository.GetFirstOrDefaultWithInclude(
                x => x.BusinessCardId == id && !x.IsDeleted);
            if (data == null)
                throw new Exception($"BusinessCard with id {id} not found");
            var businessCard = _mapper.Map(dto, data);
            businessCard.UpdatedOn = DateTime.Now;

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                userId = "Anonymous";
            businessCard.UpdatedBy = userId;

            await _unitOfWork.BusinessCardRepository.Update(businessCard);
            await _unitOfWork.Commit();

            return true;
        }

        public async Task<bool> RemoveBusinessCard(int id)
        {
            var businessCard = await _unitOfWork.BusinessCardRepository.GetFirstOrDefaultWithInclude(
                x => x.BusinessCardId == id && x.IsDeleted == false);
            if (businessCard == null)
                throw new Exception($"BusinessCard with id {id} not found");
            businessCard.UpdatedOn = DateTime.Now;
            businessCard.IsDeleted = true;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                userId = "Anonymous";
            businessCard.UpdatedBy = userId;

            await _unitOfWork.BusinessCardRepository.Update(businessCard);
            await _unitOfWork.Commit();

            return true;
        }

        public async Task<int> AddBusinessCardFromXml(string xmlContent)
        {

            var serializer = new XmlSerializer(typeof(List<BusinessCardDto>), new XmlRootAttribute("ArrayOfBusinessCardDto"));

            using (var reader = new StringReader(xmlContent))
            {
                var businessCards = (List<BusinessCardDto>)serializer.Deserialize(reader);
                int count = 0;
                foreach (var businessCard in businessCards)
                {
                    var dto = _mapper.Map<CreateBusinessCardDto>(businessCard);
                    await AddBusinessCard(dto);
                    count++;
                }
                return count;
            }
        }       

        public async Task<int> AddBusinessCardFromCsv(string csvContent)
        {
            using (var reader = new StringReader(csvContent))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {              
                MissingFieldFound = null,
                HeaderValidated = null
            }))
            {
                var records = csv.GetRecords<BusinessCard>().ToList();

                foreach (var businessCard in records)
                {
                    var dto = _mapper.Map<CreateBusinessCardDto>(businessCard);
                    await AddBusinessCard(dto);
                }

                return records.Count;
            }
        }
               
        public async Task<byte[]> ExportAllBusinessCardsToXml(BusinessCardFilterDto businessCardFilterDto)
        {
            var businessCards = await _unitOfWork.BusinessCardRepository.GetAllBusinessCards(businessCardFilterDto);

            var serializer = new XmlSerializer(typeof(List<BusinessCardDto>)); 

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);

            serializer.Serialize(writer, businessCards.Result.ToList()); 
            writer.Flush();
            return stream.ToArray();
        }

        public async Task<byte[]> ExportAllBusinessCardsToCsv(BusinessCardFilterDto businessCardFilterDto)
        {
            var businessCards = await _unitOfWork.BusinessCardRepository.GetAllBusinessCards(businessCardFilterDto);

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(businessCards.Result);
            writer.Flush();
            return stream.ToArray();
        }

       


    }
}
