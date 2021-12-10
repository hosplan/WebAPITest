using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Services
{
    public interface IUserService
    {
        /// <summary>
        /// RoleId 가
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CheckMinimumUserRole(string role);
    }

    public class UserService : IUserService
    {
        public bool CheckMinimumUserRole(string role)
        {
            try
            {
                if (Convert.ToInt32(role) >= 3) { return false; }

                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return false;
            }
        }
    }
   
}
