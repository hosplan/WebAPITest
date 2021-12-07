using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Model
{
    /// <summary>
    /// GardenUser Model
    /// CodeFirst
    /// </summary>
    public class GardenUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirm { get; set; }
        public DateTime BirthDay { get; set; }
        public string Password { get; set; }
        public bool UseSePassword { get; set; }

        public byte[] StoredSalt { get; set; }
        public string FilePath { get; set; }
        public ICollection<GardenUserRoleMap> UserRoleMaps { get; set; }
    }
    /// <summary>
    /// 유저 ViewModel
    /// </summary>
    public class UserViewModel
    {
        public string GUserName { get; set; }
        public string GUserNickName { get; set; }
        public string GUserEmail { get; set; }
        public string GUserBirthDay { get; set; }
        public bool GUserUseSePassword { get; set; }

        public string GUserFilePath { get; set; }
    }

    /// <summary>
    /// 비밀번호 Hash Model
    /// </summary>
    public class HashSalt
    {
        public string SaltPassword { get; set; }
        public byte[] Salt { get; set; }
    }

    /// <summary>
    /// 확인용 이메일 Model
    /// </summary>
    public class ConfirmEmailCode
    {
        public string AuthCode { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// 유저 이미지
    /// </summary>
    public class UserProfileImg
    {
        public string Jwt { get; set; }
        public IFormFile ProfileImg { get; set; }
    }
}
