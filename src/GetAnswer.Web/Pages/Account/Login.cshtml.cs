using System.Threading.Tasks;
using GetAnswer.DbReader.Dtos.User.Authentication;
using GetAnswer.DbReader.User;
using GetAnswer.Helpers.Cryptography;
using GetAnswer.Web.Models.User.Authentication;
using GetAnswer.Web.Pages.Account.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GetAnswer.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        [FromQuery(Name = "return-url")]
        public string ReturnUrl { get; set; }

        [BindProperty]
        public LoginForm Form { get; set; }

        
        public PageResult OnGet()
        {
            return Page();
        }
        
        public async Task<ActionResult> OnPostAsync([FromServices] AuthenticationReader authenticationReader)
        {
            if (ModelState.IsValid)
            {
                InfoForLoginPostHandler userInfo = await authenticationReader.GetInfoForLoginPostHandler(Form.Email);
                if (userInfo is null || PasswordHashHelper.CheckPasswordHash(Form.Password, userInfo.HashedPassword) == false)
                {
                    ModelState.AddModelError("", "The email or password is incorrect.");
                }
                else
                {
                    var authenticationTicketInfo = new SignInHelper.AuthenticationTicketInfo
                    {
                        UserId = userInfo.UserId,
                        FirstName = userInfo.FirstName,
                        AuthTicketInfoLastChangeUtcTime = userInfo.AuthTicketInfoLastChangeUtcTime
                    };
                    await SignInHelper.SignInAsync(authenticationTicketInfo, Form.RememberMe, HttpContext);
                }
            }

            ActionResult actionResult;

            if (!ModelState.IsValid)
            {
                actionResult = Page();
            }
            else
            {
                if (Url.IsLocalUrl(ReturnUrl))
                    actionResult = Redirect(ReturnUrl);
                else
                    actionResult = RedirectToPage("/Index");
            }

            return actionResult;
        }
    }
}