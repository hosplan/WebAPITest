using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class GUserPermissionController : ControllerBase
    {
        private readonly GardenUserContext _context;

        public GUserPermissionController(GardenUserContext context)
        {
            _context = context;
        }

        [Route("/guserpermission")]
        [HttpGet]
        public async Task<IActionResult> CheckGUserPermission()
        {
            try
            {
                var em = HttpContext.Request.Headers["Authorization"];
                var token = em.ToString();

                var handler = new JwtSecurityTokenHandler();
                var jwtValue = handler.ReadJwtToken(token);
                var claimList = jwtValue.Claims.ToList();
                var email = claimList[0].Value;
                var role = claimList[1].Value;

                return new JsonResult(new { token = true });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return new JsonResult(new { token = false });
            }
        }
    }
}
