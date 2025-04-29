namespace BusinessCardManager.Domain.Dtos
{
    public class UserFilterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int Page { get; set; } 
        public int PageSize { get; set; }
    }
}
