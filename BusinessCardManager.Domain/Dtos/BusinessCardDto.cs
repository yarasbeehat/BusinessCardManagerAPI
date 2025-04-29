
using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Domain.Dtos
{
    public class BusinessCardDto
    {
        public int BusinessCardId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? PhotoBase64 { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
