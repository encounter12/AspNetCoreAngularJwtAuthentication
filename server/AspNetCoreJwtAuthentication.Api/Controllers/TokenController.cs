using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using AspNetCoreJwtAuthentication.Models.IdentityModels;
using System.Threading.Tasks;
using AspNetCoreJwtAuthentication.Api.ViewModels;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreJwtAuthentication.Api.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private IConfiguration configuration;

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
                    //TODO: create access token and return it to client
                    //See articles: 
                    //https://auth0.com/blog/securing-asp-dot-net-core-2-applications-with-jwts/
                    //http://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api
                    //https://github.com/openiddict/openiddict-samples/blob/dev/samples/PasswordFlow/AuthorizationServer/Controllers/AuthorizationController.cs
                    //https://github.com/IdentityServer/IdentityServer4.Samples/blob/f44fd63d0508322c57c81435fccaebe1a68e9cf9/Quickstarts/6_AspNetIdentity/src/IdentityServerWithAspNetIdentity/Quickstart/Account/AccountController.cs#L99
                }
            }

            return Ok();
        }
    }
}
