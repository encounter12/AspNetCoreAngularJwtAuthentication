﻿namespace AspNetCoreJwtAuthentication.Api.ViewModels
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool RememberLogin { get; set; }
    }
}
