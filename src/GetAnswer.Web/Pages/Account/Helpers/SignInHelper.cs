using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GetAnswer.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace GetAnswer.Web.Pages.Account.Helpers
{
    public static class SignInHelper
    {
        private const int PERSISTENT_AUTHENTICATION_EXPIRES_AFTER_DAYS = 30;
        private const int NON_PERSISTENT_AUTHENTICATION_EXPIRES_AFTER_HOURS = 6;

        public static async Task SignInAsync(AuthenticationTicketInfo authenticationTicketInfo, bool persistAuthentication, HttpContext httpContext)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authenticationTicketInfo.UserId),
                new Claim(ClaimTypes.GivenName, authenticationTicketInfo.FirstName),
                new Claim(CustomAuthenticationClaims.AUTHENTICATION_RELATED_INFO_LAST_CHANGED, authenticationTicketInfo.AuthTicketInfoLastChangeUtcTime.ToString("g"))
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = persistAuthentication ?
                    DateTime.UtcNow.AddDays(PERSISTENT_AUTHENTICATION_EXPIRES_AFTER_DAYS) :
                    DateTime.UtcNow.AddHours(NON_PERSISTENT_AUTHENTICATION_EXPIRES_AFTER_HOURS),
                IsPersistent = persistAuthentication
            };

            await httpContext.SignInAsync(claimsPrincipal, authenticationProperties);
        }


        public class AuthenticationTicketInfo
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public DateTime AuthTicketInfoLastChangeUtcTime { get; set; }
        }
    }
}