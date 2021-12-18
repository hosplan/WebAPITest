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
        private readonly IJWTService _jwtService;

        public GUserMemberShipController(IHashService hashService, GardenUserContext context, IJWTService jwtService)
        {
            _hashService = hashService;
            _context = context;
            _jwtService = jwtService;
        }
        
        /// <summary>
        /// 유저 회원가입
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/membership")]
        public async Task<IActionResult> CreateGardenUser([FromForm] GardenUser gardenUser)
        {
            try
            {
                HashSalt hashSalt = _hashService.EncryptionPassword(gardenUser.Password);

                gardenUser.Password = hashSalt.SaltPassword;
                gardenUser.StoredSalt = hashSalt.Salt;

                _context.Add(gardenUser);
                await _context.SaveChangesAsync();

                await CreateGardenUserRole(gardenUser.Id);

                return Ok(new { token = true, jwt = _jwtService.GenerateJWT(gardenUser.Email, 4) });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        /// <summary>
        /// UserRoleMap 생성
        /// </summary>
        /// <param name="guserId"></param>
        /// <returns></returns>
        private async Task<bool> CreateGardenUserRole(int guserId)
        {
            try
            {
                GardenUserRoleMap map = new GardenUserRoleMap();
                map.UserId = guserId;
                map.RoleId = 4;

                _context.Add(map);
                await _context.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return false;
            }
        }

        [HttpPost]
        [Route("/checkemail")]
        public IActionResult CheckEmail([FromForm] GardenUser guser)
        {
            try
            {
                GardenUser gUser = _context.GardenUser.FirstOrDefault(g => g.Email == guser.Email);
                if(gUser != null) { return Ok(new { token = false , message = "이미 가입된 이메일 이네요."}); }

                return Ok(new { token = true, message = "확인완료" });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false , message = "오류가 발생했습니다. 잠시후에 다시 시도 해주세요!"});
            }
        }
    }
}
