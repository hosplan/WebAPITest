using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Model
{
    public class GardenUserRoleMap
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("UserId")]
        public GardenUser GardenUser { get; set; }
        [ForeignKey("RoleId")]
        public GardenRole GardenRole { get; set; }
    }
}
