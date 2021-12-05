using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Model
{
    public class GardenRole
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; }
        public ICollection<GardenUserRoleMap> UserRoleMaps { get; set; }
    }
}
