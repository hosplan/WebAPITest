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

       

        /// <summary>
        /// viewModel 정보를 통한 GardenUser info 업데이트
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/guser")]
        public async Task<IActionResult> UserInfoUpdate([FromBody] UserViewModel viewModel)
        {
            try
            {
                GardenUser gUser = GetGUserThroughEmail(viewModel.GUserEmail);

                if(gUser == null) { return Ok( new { token = false }); }

                gUser.BirthDay = Convert.ToDateTime(viewModel.GUserBirthDay);
                gUser.NickName = viewModel.GUserNickName;
                gUser.UseSePassword = viewModel.GUserUseSePassword;
                gUser.Name = viewModel.GUserName;

                await UpdateUser(gUser);

                return Ok(new { token = true});
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
            catch(Exception ex)
            {
                string error = ex.Message;
                throw;
            }

        }
    }
}
