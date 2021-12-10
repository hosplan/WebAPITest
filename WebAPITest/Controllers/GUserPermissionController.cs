using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;
using WebAPITest.Services;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class GUserPermissionController : ControllerBase
    {
        private readonly GardenUserContext _context;
        private readonly IUserService _userService;
        private readonly IJWTService _jwtService;

        public GUserPermissionController(GardenUserContext context, IUserService userService, IJWTService jwtService)
        {
            _context = context;
            _userService = userService;
            _jwtService = jwtService;
        }

        [Route("/guserpermission")]
        [HttpGet]
        public IActionResult CheckGUserPermission()
        {
            try
            {                
                JwtSecurityToken jwtValue = _jwtService.DecodeJwt(HttpContext.Request.Headers["Authorization"].ToString());
                var claimList = jwtValue.Claims.ToList();
                return new JsonResult(new { token = _userService.CheckMinimumUserRole(claimList[1].Value) });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return new JsonResult(new { token = false });
            }
        }
    }
}
