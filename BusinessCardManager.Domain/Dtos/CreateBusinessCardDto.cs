using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Domain.Dtos
{
    public class CreateBusinessCardDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? PhotoBase64 { get; set; }
        public int UserId { get; set; }
    }
}
