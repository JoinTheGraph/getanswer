using System.Linq;
using System.Threading.Tasks;
using GetAnswer.DbReader.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GetAnswer.Web.Infrastructure
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private AuthenticationReader _authenticationReader;

        public CustomCookieAuthenticationEvents(AuthenticationReader authenticationReader)
        {
            _authenticationReader = authenticationReader;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            string loggedInUserId = context.Principal.Identity.Name;
            string authenticationRelatedInfoLastChangeDateFromTicket =
                context.Principal.Claims
                                 .Single(claim => claim.Type == CustomAuthenticationClaims.AUTHENTICATION_RELATED_INFO_LAST_CHANGED)
                                 .Value;

            string authenticationRelatedInfoLastChangeDateFromDb = (await _authenticationReader.GetAuthTicketInfoLastChangeUtcTimeAsync(loggedInUserId)).ToString("g");

            if (authenticationRelatedInfoLastChangeDateFromTicket != authenticationRelatedInfoLastChangeDateFromDb)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync();
            }
        }
    }
}