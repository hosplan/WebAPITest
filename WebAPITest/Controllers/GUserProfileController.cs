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
    public class GUserProfileController : ControllerBase
    {
        private readonly GardenUserContext _context;
        private readonly IJWTService _jwtService;

        public GUserProfileController(GardenUserContext context, IJWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost]
        [Route("/profile")]
        public async Task<IActionResult> EnrollUserProfileImg([FromForm] Model.UserProfileImg profile)
        {
            try
            {
                int guserId = await GetGardenUserId(profile.Jwt);

                return Ok(new { token = true });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        private async Task<int> GetGardenUserId(string jwt)
        {
            try
            {
                var token = _jwtService.DecodeJwt(jwt);

                return 0;
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                throw;                
            }
        }
    }
}
