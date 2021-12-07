using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class GUserEmailConfirm : ControllerBase
    {
        private readonly GardenUserContext _context;

        public GUserEmailConfirm(GardenUserContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 이메일 인증 유무 업데이트
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/comfirmation")]
        public async Task<IActionResult> UserEmailConfirmUpdate([FromBody] GardenUser gardenUser)
        {
            try
            {
                GardenUser gUser = _context.GardenUser.FirstOrDefault(g => g.Email == gardenUser.Email);
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
