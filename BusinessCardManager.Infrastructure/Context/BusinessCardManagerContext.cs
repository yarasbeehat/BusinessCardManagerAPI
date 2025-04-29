using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessCardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessCardManager.Infrastructure.Context
{
    public class BusinessCardManagerContext : DbContext
    {

        private readonly IConfiguration configuration;

        public BusinessCardManagerContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
        public DbSet<BusinessCard> BusinessCards { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
