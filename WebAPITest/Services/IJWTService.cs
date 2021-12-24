using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Services
{
    public interface IJWTService
    {
        /// <summary>
        /// Jwt 생성
        /// </summary>
        /// <param name="email"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GenerateJWT(GardenUser email, int roleId);

        /// <summary>
        /// Jwt decode
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public JwtSecurityToken DecodeJwt(string jwt);

        /// <summary>
        /// Jwt 내부의 Claimlist 얻기
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public List<Claim> GetClaimList(string jwt);


        public string GetEmail(List<Claim> claimList);

        public string GetRoleId(List<Claim> claimList);
    }

    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;
        public JWTService(IConfiguration config)
        {
            _config = config;
        }

        public string GetEmail(List<Claim> claimList)
        {
            return claimList[0].Value;
        }

        public string GetRoleId(List<Claim> claimList)
        {
            return claimList[1].Value;
        }

        public JwtSecurityToken DecodeJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(jwt);
        }

        public List<Claim> GetClaimList(string jwt)
        {
            JwtSecurityToken jwtSecurityToken = DecodeJwt(jwt);
            return jwtSecurityToken.Claims.ToList();
        }


        public string GenerateJWT(GardenUser guser, int roleId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("eml", guser.Email),
                new Claim("rol", roleId.ToString()),
                new Claim("jti", Guid.NewGuid().ToString()),
                new Claim("gky", guser.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
              );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
