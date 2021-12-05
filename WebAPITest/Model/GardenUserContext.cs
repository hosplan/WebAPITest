using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Model
{
    public class GardenUserContext : DbContext
    {
        public GardenUserContext(DbContextOptions<GardenUserContext> options)
            : base(options)
        {
        }
        
        public DbSet<GardenUser> GardenUser { get; set; }
        public DbSet<GardenRole> GardenRole { get; set; }
        public DbSet<GardenUserRoleMap> UserRoleMaps { get; set; }
        
    }
}
