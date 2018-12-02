using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using AspNetCoreJwtAuthentication.Api.ViewModels;
using AspNetCoreJwtAuthentication.Models.IdentityModels;
using AspNetCoreJwtAuthentication.Models.InfrastructureModels;

namespace AspNetCoreJwtAuthentication.Api.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public TokenController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(
                    model.Username, 
                    model.Password, 
                    model.RememberLogin, 
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    ApplicationUser applicationUser = await userManager.FindByNameAsync(model.Username);

                    var userModel = new UserModel()
                    {
                        Name = applicationUser.UserName,
                        Email = applicationUser.Email,
                        Birthdate = applicationUser.Birthdate
                    };

                    var tokenString = await this.BuildToken(userModel, applicationUser);

                    return Ok(new { token = tokenString });

                    //Generating JWT token articles: 
                    //https://auth0.com/blog/securing-asp-dot-net-core-2-applications-with-jwts/
                    //http://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api
                    //https://github.com/openiddict/openiddict-samples/blob/dev/samples/PasswordFlow/AuthorizationServer/Controllers/AuthorizationController.cs
                    //https://github.com/IdentityServer/IdentityServer4.Samples/blob/f44fd63d0508322c57c81435fccaebe1a68e9cf9/Quickstarts/6_AspNetIdentity/src/IdentityServerWithAspNetIdentity/Quickstart/Account/AccountController.cs#L99
                }
            }

            return BadRequest();
        }

        private async Task<string> BuildToken(UserModel user, ApplicationUser applicationUser)
        {
            JwtSettings jwtSettings = this.configuration.GetSection("JwtSettings").Get<JwtSettings>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoleClaims = await this.GetUserRoleClaims(applicationUser);

            claims.AddRange(userRoleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                jwtSettings.Issuer,
                jwtSettings.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<Claim>> GetUserRoleClaims(ApplicationUser applicationUser)
        {
            IList<string> userRoles = await userManager.GetRolesAsync(applicationUser);

            var userRoleClaims = userRoles
                .Select(role => new Claim(ClaimTypes.Role, role))
                .ToList();

            return userRoleClaims;
        }
    }
}
