

using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Domain.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public ICollection<BusinessCardDto> BusinessCards { get; set; } = new List<BusinessCardDto>();
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
