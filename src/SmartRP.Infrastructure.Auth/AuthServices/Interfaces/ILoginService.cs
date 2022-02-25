using System.Threading.Tasks;

namespace SmartRP.Infrastructure.Auth
{
	public interface ILoginService
	{
		Task SignOn();
		Task SignOut();
        string GetCurrentLoginIdentityID();
        bool EmailVerified(string loginIdentityID);
    }
}