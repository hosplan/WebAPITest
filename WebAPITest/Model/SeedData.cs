using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Model
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using(var context = new GardenUserContext(
                serviceProvider.GetRequiredService<DbContextOptions<GardenUserContext>>()))
            {
                if(context.GardenRole.Any())
                {
                    return;
                }

                context.GardenRole.AddRange(
                    new GardenRole
                    {
                        RoleName = "System"
                    },
                    new GardenRole
                    {
                        RoleName = "Manager"
                    },
                    new GardenRole
                    {
                        RoleName = "User"
                    },
                    new GardenRole
                    {
                        RoleName = "Guest"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
