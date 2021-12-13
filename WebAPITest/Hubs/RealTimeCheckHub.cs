using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;

namespace WebAPITest.Hubs
{
    public class RealTimeCheckHub : Hub
    {
        /// <summary>
        /// Dictionary<email, authorizationCode>
        /// </summary>
        private static Dictionary<string, string> ConfirmEmailDic = new Dictionary<string, string>();
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        /// <summary>
        /// 회원인증용 이메일 저장
        /// </summary>
        /// <param name="email"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        public bool EnrollmentConfirmEmail(string email, string authCode)
        {
            try
            {
                ConfirmEmailDic.Add(email, authCode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 이메일 인증 시간이 초과 되었을시 ConfirmEmailDic에서 해당 이메일 정보 삭제
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ResetEmailconfirm(string email)
        {
            try
            {
                return ConfirmEmailDic.Remove(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 회원인증 완료 체크
        /// </summary>
        /// <param name="email"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        public bool ConfirmAuthCode(string email, string authCode)
        {
            try
            {
                if (!ConfirmEmailDic.ContainsKey(email)) return false;

                ConfirmEmailDic.Remove(email);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
