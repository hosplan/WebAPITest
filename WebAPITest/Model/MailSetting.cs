using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Model
{
    public class MailSetting
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class ConfirmEmail
    {
        public string Email { get; set; }
        public string AuthCode { get; set; }
    }
}
