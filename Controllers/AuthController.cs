using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFFApi.Controllers
{
    [ApiController]
    [Route("api")]

    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/login
        [HttpGet]
        [Route("login")]
        public IResult Login(string returnUrl = "/")
        {
            try
            {
                if (!IsValidRedirectUrl(returnUrl))
                {
                    return Results.BadRequest("Invalid request");
                }

                return Results.Challenge(new AuthenticationProperties
                {
                    IsPersistent = true,
                    RedirectUri = returnUrl
                }, new List<string>() { "OpenIdConnect" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private bool IsValidRedirectUrl(string redirectUrl)
        {
            var allowedOrigins = _configuration.GetSection("AllowedOrigins").Get<List<string>>();
            return allowedOrigins.Contains(redirectUrl);
        }

        // GET: api/logout
        [Authorize]
        [HttpGet]
        [Route("logout")]
        public async Task<IResult> Logout()
        {
            await HttpContext.SignOutAsync();

            var postSignOutRedirect = _configuration["OpenIdConnect:PostSignOutRedirect"];

            return new SignOutResult("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = postSignOutRedirect
            });
        }

        // GET: api/user
        [HttpGet]
        [Route("user")]
        public IResult GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = ((ClaimsIdentity)this.User.Identity).Claims.Select(c =>
                                new { type = c.Type, value = c.Value })
                                .ToArray();

                return Results.Json(new { isAuthenticated = true, claims = claims });
            }

            return Results.Json(new { isAuthenticated = false });
        }
    }
}