using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 로그인된 유저 정보 가져오기
        /// </summary>
        /// <param name="currentUser">현재 로그인된 유저</param>
        /// <returns>GardenUser</returns>
        GardenUser GetLoginUser(ClaimsPrincipal currentUser);
        /// <summary>
        /// 유저 정보 업데이트
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <returns></returns>
        Task<bool> UpdateLoginUser(GardenUser gardenUser);
        /// <summary>
        /// 유저 등급 정보 업데이트
        /// </summary>
        /// <param name="userId">유저 아이디</param>
        /// <param name="gradeId">등급 아이디</param>
        /// <returns></returns>
        Task<bool> UpdateUserRoleMap(int userId, int roleId);
        /// <summary>
        /// 로그인한 유저의 뷰 모델 가져오기
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        UserViewModel GetUserViewModel(ClaimsPrincipal currentUser);
        /// <summary>
        /// 유저 닉네임 변경
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <param name="nickName">변경할 유저의 닉네임 정보</param>
        /// <returns></returns>
        Task<bool> UpdateUserNickName(GardenUser gardenUser, string nickName);
        /// <summary>
        /// 유저 이름 변경
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <param name="name">변경할 유저의 이름 정보</param>
        /// <returns></returns>
        Task<bool> UpdateUserName(GardenUser gardenUser, string name);
        /// <summary>
        /// 유저 생년월일 변경
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <param name="birthday">변경할 유저의 생년월일 정보</param>
        /// <returns></returns>
        Task<bool> UpdateUserBirthDay(GardenUser gardenUser, DateTime birthday);
        /// <summary>
        /// 유저의 2차비밀번호 사용 여부
        /// </summary>
        /// <param name="gardenUser"></param>
        /// <param name="useSePassword">사용여부에 대한 on/off</param>
        /// <returns></returns>
        Task<bool> UpdateUserUseSePassword(GardenUser gardenUser, bool useSePassword);
    }

    public class UserService : IUserService
    {
        private readonly GardenUserContext _context;

        public UserService(GardenUserContext context)
        {
            _context = context;
        }

        public UserViewModel GetUserViewModel(ClaimsPrincipal currentUser)
        {           
            try
            {
                UserViewModel userViewModel = new UserViewModel();

                if (!currentUser.HasClaim(z => z.Type == "eml")) return userViewModel;

                string email = currentUser.FindFirst(c => c.Type == "eml").Value;
                GardenUser gardenUser = _context.GardenUser.FirstOrDefault(g => g.Email == email);

                userViewModel = new UserViewModel
                {
                    GUserEmail = gardenUser.Email,
                    GUserNickName = gardenUser.NickName,
                    GUserName = gardenUser.Name,
                    GUserBirthDay = gardenUser.BirthDay.ToString("yyyy-MM-dd"),
                    GUserUseSePassword = gardenUser.UseSePassword,
                    GUserFilePath = gardenUser.FilePath
                };

                return userViewModel;
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public async Task<bool> UpdateUserNickName(GardenUser gardenUser, string nickName)
        {
            try
            {
                gardenUser.NickName = nickName;
                _context.Update(gardenUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex.Message);
                return false;
            }

        }

        public async Task<bool> UpdateUserName(GardenUser gardenUser, string name)
        {
            try
            {
                gardenUser.Name = name;
                _context.Update(gardenUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex.Message);
                return false;
            }

        }

        public async Task<bool> UpdateUserBirthDay(GardenUser gardenUser, DateTime birthday)
        {
            try
            {
                gardenUser.BirthDay = birthday;
                _context.Update(gardenUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex.Message);
                return false;
            }

        }

        public async Task<bool> UpdateUserUseSePassword(GardenUser gardenUser, bool useSePassword)
        {
            try
            {
                gardenUser.UseSePassword = useSePassword;
                _context.Update(gardenUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex.Message);
                return false;
            }

        }

        public async Task<bool> UpdateUserRoleMap(int userId, int roleId)
        {
            try
            {
                //3 -> User
                GardenUserRoleMap userRoleMap = _context.UserRoleMaps.FirstOrDefault(u => u.UserId == userId);
                userRoleMap.RoleId = roleId;

                _context.Update(userRoleMap);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdateLoginUser(GardenUser gardenUser)
        {
            try
            {
                _context.Update(gardenUser);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException cuurencyEx)
            {
                //저장 하는 동안 동시성 위반이 발생한다.
                string error = cuurencyEx.ToString();
                return false;
            }
            catch (DbUpdateException updateEx)
            {
                //데이터베이스에 저장 하는 동안 오류가 발생한 경우
                string error = updateEx.ToString();
                return false;
            }
        }

        public GardenUser GetLoginUser(ClaimsPrincipal currentUser)
        {
            GardenUser gardenUser = new GardenUser();
            try
            {
                if (!currentUser.HasClaim(z => z.Type == "eml")) return gardenUser;
                string email = currentUser.FindFirst(c => c.Type == "eml").Value;

                return gardenUser = _context.GardenUser.FirstOrDefault(g => g.Email == email);
            }
            catch (Exception ex)
            {
                string error = Convert.ToString(ex.Message);
                return gardenUser;
            }
        }
    }
}
