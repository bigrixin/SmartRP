using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Threading.Tasks;
using System.Web;

namespace SmartRP.Infrastructure.Auth
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public LoginService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public Task SignOn()
        {
            return null;
        }
        public Task SignOut()
        {
            return null;
        }

        public string GetCurrentLoginIdentityID()
        {
            try
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
            catch (Exception e)
            {
                var msg = e.Message;

                return null;
            }
        }

        public bool EmailVerified(string loginIdentityID)
        {
         
            return _userManager.IsEmailConfirmed(loginIdentityID);
        }
    }
}
