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

        private string GetJWT()
        {
            return HttpContext.Request.Headers["Authorization"].ToString();
        }

        private UserViewModel GetUserViewModel(string email)
        {
            UserViewModel viewModel = new UserViewModel();

            try
            {
                GardenUser guser = _context.GardenUser.FirstOrDefault(g => g.Email == email);

                viewModel = new UserViewModel
                {
                    GUserEmail = guser.Email,
                    GUserNickName = guser.NickName,
                    GUserName = guser.Name,
                    GUserBirthDay = guser.BirthDay.ToString("yyyy-MM-dd"),
                    GUserUseSePassword = guser.UseSePassword,
                    GUserFilePath = guser.FilePath
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return viewModel;
            }
        }


        /// <summary>
        /// viewModel 정보를 통한 GardenUser info 업데이트
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/guser")]
        public async Task<IActionResult> UserInfoUpdate([FromForm] UserViewModel viewModel)
        {
            try
            {
                GardenUser gUser = GetGUserThroughEmail(viewModel.GUserEmail);

                if (gUser == null) { return Ok(new { token = false }); }

                gUser.BirthDay = Convert.ToDateTime(viewModel.GUserBirthDay);
                gUser.NickName = viewModel.GUserNickName;
                gUser.UseSePassword = viewModel.GUserUseSePassword;
                gUser.Name = viewModel.GUserName;

                await UpdateUser(gUser);

                return Ok(new { token = true });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        /// <summary>
        /// 이메일 주소를 통해 유저의 정보 얻기
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private GardenUser GetGUserThroughEmail(string email)
        {
            return _context.GardenUser.FirstOrDefault(g => g.Email == email);
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

        [HttpGet]
        [Route("/guser")]
        public IActionResult GetUserInfo()
        {
            try
            {
                string jwt = GetJWT();
                if (string.IsNullOrEmpty(jwt)) { return Ok(new { token = false }); }
                string email = _jwtService.GetEmail(_jwtService.GetClaimList(jwt));

                return Ok(new { data = GetUserViewModel(email) });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { data = false });
            }
        }
    }
}
