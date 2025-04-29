using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Domain.Entities
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public List<RoleUser> RoleUsers { get; set; } = new();
    }
}
