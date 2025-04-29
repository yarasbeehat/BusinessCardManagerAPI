using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardController : Controller
    {
        private readonly IBusinessCardService _businessCardService;

        public BusinessCardController(IBusinessCardService businessCardService)
        {
            _businessCardService = businessCardService;
        }        

        [HttpPost("GetAllBusinessCards")]
        public async Task<IActionResult> GetAllBusinessCards([FromBody] BusinessCardFilterDto filter)
            => Ok(await _businessCardService.GetAllBusinessCards(filter));

        [HttpGet("GetBusinessCardById")]
        public async Task<IActionResult> GetBusinessCardById(int id)
            => Ok(await _businessCardService.GetBusinessCardById(id));

        [HttpPost("AddBusinessCard")]
        public async Task<IActionResult> AddBusinessCard([FromBody] CreateBusinessCardDto dto)
        {
            var id = await _businessCardService.AddBusinessCard(dto);
            return Ok($"BusinessCard with Id {id} was added successfully");
        }

        [HttpPut("UpdateBusinessCard/{id}")]
        public async Task<IActionResult> UpdateBusinessCard(int id, [FromBody] CreateBusinessCardDto dto)
            => Ok(await _businessCardService.UpdateBusinessCard(id, dto));

        [HttpDelete("RemoveBusinessCard/{id}")]
        public async Task<IActionResult> RemoveBusinessCard(int id)
            => Ok(await _businessCardService.RemoveBusinessCard(id));

        [HttpPost("ImportBusinessCardFromXml")]
        public async Task<IActionResult> ImportBusinessCardFromXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var xmlContent = await reader.ReadToEndAsync();

            var result = await _businessCardService.AddBusinessCardFromXml(xmlContent);
            return Ok(new { Success = true });
        }

        [HttpPost("ImportBusinessCardFromCsv")]
        public async Task<IActionResult> ImportBusinessCardFromCsv(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest("CSV file is empty or missing.");
            }

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var csvContent = await stream.ReadToEndAsync();
                var result = await _businessCardService.AddBusinessCardFromCsv(csvContent);
                return Ok(new { Success = true, Imported = result });
            }
        }

        [HttpPost("ExportAllBusinessCardsToXml")]
        public async Task<IActionResult> ExportAllBusinessCardsToXml([FromBody] BusinessCardFilterDto filter)
        {
            var xmlBytes = await _businessCardService.ExportAllBusinessCardsToXml(filter);
            return File(xmlBytes, "application/xml", "BusinessCards.xml");
        }

        [HttpPost("ExportAllBusinessCardsToCsv")]
        public async Task<IActionResult> ExportAllBusinessCardsToCsv([FromBody] BusinessCardFilterDto businessCardFilterDto)
        {
            var content = await _businessCardService.ExportAllBusinessCardsToCsv(businessCardFilterDto);
            return File(content, "text/csv", "business_cards.csv");
        }

    }
}
