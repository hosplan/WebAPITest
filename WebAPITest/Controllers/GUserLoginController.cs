using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;
using WebAPITest.Services;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class GUserLoginController : ControllerBase
    {
        private readonly GardenUserContext _context;
        private readonly IHashService _hashService;
        private readonly IJWTService _jwtService;
        public GUserLoginController(GardenUserContext context, 
                                    IHashService hashService,
                                    IJWTService jwtService)
        {
            _context = context;
            _hashService = hashService;
            _jwtService = jwtService;
        }

        [Route("/guserLogin/login")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] GardenUser login)
        {
            try
            {
                GardenUser gUser = GetLoginUser(login.Email);
                if (gUser == null)
                {
                    return Ok(new { token = false });
                }
                else if (CheckLoginUserInfo(login, gUser.StoredSalt) == false)
                {
                    return Ok(new { token = false });
                }

                return Ok(new
                {
                    token = _jwtService.GenerateJWT(gUser.Email, gUser.UserRoleMaps.First().RoleId)
                });
            }
            catch(Exception ex)
            {
                return Ok(new { token = false, err = ex.Message });
            }
        }

        private bool CheckLoginUserInfo(GardenUser gUser, byte[] storedSalt)
        {
            GardenUser isSucceed = _context.GardenUser
                                         .AsNoTracking()
                                         .SingleOrDefault(z => z.Email == gUser.Email
                                                       && z.Password == _hashService.StoredSaltPassword(gUser.Password, storedSalt));

            if(isSucceed == null) { return false; }

            return true;
        }

        /// <summary>
        /// 회원 검사 - 이메일 이 있는지
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private GardenUser GetLoginUser(string email)
        {
            return _context.GardenUser.Include(g => g.UserRoleMaps)
                                        .ThenInclude(m => m.GardenRole)
                                        .AsNoTracking()
                                        .FirstOrDefault(g => g.Email == email);
        }


    }
}
