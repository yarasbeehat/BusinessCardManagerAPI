using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Domain.Dtos
{
    public class BusinessCardFilterDto
    {
        public int? BusinessCardId { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? UserId { get; set; }
        public int PageIndex { get; set; } 
        public int PageSize { get; set; }
    }
}
