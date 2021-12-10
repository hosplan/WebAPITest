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
    public class GUserMemberShipController : ControllerBase
    {
        private readonly IHashService _hashService;
        private readonly GardenUserContext _context;

        public GUserMemberShipController(IHashService hashService, GardenUserContext context)
        {
            _hashService = hashService;
            _context = context;
        }
        
        /// <summary>
        /// 유저 회원가입
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/membership")]
        public async Task<IActionResult> CreateGardenUser([FromBody] GardenUser gardenUser)
        {
            try
            {
                HashSalt hashSalt = _hashService.EncryptionPassword(gardenUser.Password);

                gardenUser.Password = hashSalt.SaltPassword;
                gardenUser.StoredSalt = hashSalt.Salt;

                //_context.Add(gardenUser);
                //await _context.SaveChangesAsync();

                return Ok(new { token = true });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }


        [HttpPost]
        [Route("/checkemail")]
        public IActionResult CheckEmail([FromBody] GardenUser guser)
        {
            try
            {
                GardenUser gUser = _context.GardenUser.FirstOrDefault(g => g.Email == guser.Email);
                if(gUser != null) { return Ok(new { token = false }); }

                return Ok(new { token = true });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }
    }
}
