using System.Net;
using System.Web.Mvc;
using SmartRP.Domain;
using SmartRP.Infrastructure.Auth;
using RazorEngine.Templating;
using SmartRP.Domain.Service;

namespace SmartRP.Controllers
{
	[Authorize]
	public class MyAccountController : Controller
	{
		#region Fields

		private readonly ILoginService _loginService;
		private readonly IUserService _userService;
		private readonly IProjectService _projectService;

		#endregion

		#region Ctor

		public MyAccountController(ILoginService loginService, IUserService userService, IProjectService projectService)
		{
			_loginService = loginService;
			_userService = userService;
			_projectService = projectService;
		}

		#endregion

		#region Actions

		// GET: MyAccount
		public ActionResult Index()
		{
			string userType = _userService.GetCurrentRole(User.Identity.Name);
			if (userType == "Admin")
				return RedirectToAction("Info", "Manage", new { usertype = userType });
			else
			{
				User user = _userService.GetUserIDByEmail(User.Identity.Name);
				string userID = user.LoginIdentityID;
				if (_userService.IsNewUser(User.Identity.Name))
					return RedirectToAction("Update-Profile", userType, new { userid = userID });
				else
					return RedirectToAction("MyAccount", userType);
			}
		}

		[Authorize(Roles = "Admin")]
		public ActionResult ViewUsers()
		{
			var users = _userService.GetAllUser();
			return View(users);
		}

		// GET: MyAccount/Details/5
		[Authorize(Roles = "Admin")]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = _userService.FindUser(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		#endregion
	}
}
