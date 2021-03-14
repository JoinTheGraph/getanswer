using GetAnswer.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace GetAnswer.Web.StartupConfiguration
{
    public static class CookieAuthenticationConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(
                        cookieAuthenticationOptions =>
                        {
                            cookieAuthenticationOptions.LoginPath = "/account/login";
                            cookieAuthenticationOptions.ReturnUrlParameter = "return-url";
                            cookieAuthenticationOptions.EventsType = typeof(CustomCookieAuthenticationEvents);
                        }
                    );

            services.AddScoped<CustomCookieAuthenticationEvents>();
        }
    }
}