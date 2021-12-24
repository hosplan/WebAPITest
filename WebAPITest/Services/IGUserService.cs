using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Services
{
    public interface IGUserService
    {
        /// <summary>
        /// 실제 회원인지, 그리고 회원의 이메일 과 비밀번호가 일치한지
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public bool AuthenticateUser(GardenUser loginUser);
        /// <summary>
        /// JWT 토큰 생성
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public string GetJwtToken(GardenUser loginUser);
    }

    public class GUserService : IGUserService
    {
        private readonly GardenUserContext _context;
        private readonly IHashService _hashService;
        private readonly IJWTService _jwtService;
        public GUserService(GardenUserContext context, IHashService hashService, IJWTService jwtService)
        {
            _context = context;
            _hashService = hashService;
            _jwtService = jwtService;
        }

        public string GetJwtToken(GardenUser loginUser)
        {
            try
            {
                return _jwtService.GenerateJWT(loginUser, loginUser.UserRoleMaps.First().RoleId);
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public bool AuthenticateUser(GardenUser loginUser)
        {
            //현재 이메일이 있는지 확인
            GardenUser gUser = _context.GardenUser.SingleOrDefault(z => z.Email == loginUser.Email);
            if (gUser == null) return false;

            //이메일과 비밀번호가 일치하는지 확인
            GardenUser isSucceed = _context.GardenUser
                                           .SingleOrDefault(z => z.Email == loginUser.Email
                                                            && z.Password == _hashService.StoredSaltPassword(loginUser.Password, gUser.StoredSalt));
            if (isSucceed == null) return false;

            return true;
        }
    }
}
