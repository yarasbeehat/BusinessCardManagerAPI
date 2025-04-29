using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Domain.Dtos
{
    public class AuthenticationDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
    }
}
