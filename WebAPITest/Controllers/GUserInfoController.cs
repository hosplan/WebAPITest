using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;
using WebAPITest.Services;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class GUserInfoController : ControllerBase
    {
        private readonly GardenUserContext _context;
       
        public GUserInfoController(GardenUserContext context, IUserService userService)
        {
            _context = context;
        }

       

    }
}
