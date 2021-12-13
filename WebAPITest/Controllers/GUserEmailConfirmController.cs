using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Model;
using WebAPITest.Services;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class GUserEmailConfirmController : ControllerBase
    {
        private readonly GardenUserContext _context;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        private readonly IJWTService _jwtService;
        public GUserEmailConfirmController(GardenUserContext context, IConfiguration config, IMailService mailService, IJWTService jwtService)
        {
            _context = context;
            _config = config;
            _mailService = mailService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Authorization 값 가져오기 - jwt
        /// </summary>
        /// <returns></returns>
        private string GetJWT()
        {
            return HttpContext.Request.Headers["Authorization"].ToString();
        }
        /// <summary>
        /// 인증메일 보내기
        /// </summary>
        /// <param name="guser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/send_confirm_email")]
        public IActionResult SendConfirmEmail()
        {            
            try
            {
                string jwt = GetJWT();

                if(string.IsNullOrEmpty(jwt)) { return Ok(new { token = false }); }

                string email = _jwtService.GetEmail(_jwtService.GetClaimList(jwt));
                MailRequest mailcontent = GenerateMailContent(email);
                Task<bool> isSucceed = _mailService.SendEmailAsync(mailcontent);

                if (isSucceed.Result == false) { return Ok(new { token = false }); }

                return Ok(new { token = true, data = new { email = email, authCode = mailcontent.Body } });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        /// <summary>
        /// 메일 내용 생성
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private MailRequest GenerateMailContent(string email)
        {
            return new MailRequest
            {
                ToEmail = email,
                Subject = "Garden 회원가입 인증코드 입니다.",
                Body = GenerateAuthCode()
            };
        }

        /// <summary>
        /// 인증코드 생성
        /// </summary>
        /// <returns></returns>
        private string GenerateAuthCode()
        {
            StringBuilder authCode = new StringBuilder();
            Random random = new Random();
            for(int i = 0; i < 6; i++)
            {
                int num = random.Next(10);
                authCode.Append(num);
            }
            return authCode.ToString();
        }

        /// <summary>
        /// 이메일 인증 유무 업데이트
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/comfirmation")]
        public async Task<IActionResult> UserEmailConfirmUpdate()
        {
            try
            {
                string jwt = GetJWT();
                if(string.IsNullOrEmpty(jwt)) { return Ok(new { token = false }); }

                GardenUser gUser = _context.GardenUser.FirstOrDefault(g => g.Email == _jwtService.GetEmail(_jwtService.GetClaimList(jwt)));
                if (gUser == null) return Ok(new { token = false });

                gUser.EmailConfirm = true;
                await UpdateUser(gUser);
                await UpdateUserRoleMap(gUser.Id, 3);

                return Ok(new { token = true });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        /// <summary>
        /// GardenUser - Update
        /// </summary>
        /// <param name="guser"></param>
        /// <returns></returns>
        private async Task<bool> UpdateUser(GardenUser gUser)
        {
            try
            {
                _context.Update(gUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }

        }

        /// <summary>
        /// 유저 - Role Map 업데이트
        /// </summary>
        /// <param name="gUserId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateUserRoleMap(int gUserId, int roleId)
        {
            try
            {
                //3 -> User
                GardenUserRoleMap map = _context.UserRoleMaps.FirstOrDefault(m => m.UserId == gUserId);
                map.RoleId = roleId;

                _context.Update(map);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }
    }
}
