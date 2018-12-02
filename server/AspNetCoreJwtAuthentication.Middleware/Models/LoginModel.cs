using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreJwtAuthentication.Middleware.Models
{
    public class LoginModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberLogin { get; set; }
    }
}
