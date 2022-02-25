using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SmartRP.Domain.Service;
using SmartRP.Models;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SmartRP.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{

		#region Fields

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private readonly UserManager<IdentityUser> _userManager;
		private UserManager<IdentityUser> UserManager => _userManager;

		private readonly SignInManager<IdentityUser, string> _signInManager;
		private SignInManager<IdentityUser, string> SignInManager => _signInManager;

		private readonly RoleManager<IdentityRole> _roleManager;
		private RoleManager<IdentityRole> RoleManager => _roleManager;

		private readonly IAuthenticationManager _authenticationManager;
		private IAuthenticationManager AuthenticationManager => _authenticationManager;

		private readonly IUserService _userService;

		#endregion

		#region Ctor

		public AccountController(
				UserManager<IdentityUser> userManager,
				RoleManager<IdentityRole> roleManager,
				SignInManager<IdentityUser, string> signInManager,
				IAuthenticationManager authenticationManager,
				IUserService userService)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_authenticationManager = authenticationManager;
			_userService = userService;
		}

		#endregion

		#region Actions

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "MyAccount");

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = await UserManager.FindAsync(model.Email, model.Password);
			if (user == null)
				user = await UserManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				if (!await UserManager.IsEmailConfirmedAsync(user.Id))
					return RedirectToAction("ResendEmail", "Account", new { Email = model.Email });
				AddNewUser(user.Id);
			}

			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					if (returnUrl == null)
						return RedirectToAction("Index", "MyAccount");
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:

					if (user != null)
					{
						bool passhash = false;
						passhash = await UserManager.CheckPasswordAsync(user, model.Password);
						if (passhash)
							return RedirectToLocal(returnUrl);
					}

					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = new IdentityUser { UserName = model.Email, Email = model.Email };
					var result = await UserManager.CreateAsync(user, model.Password);

					if (result.Succeeded)
					{
						// send email to ask the newly registered user to 'confirm his/her email address' 
						string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
						await UserManager.SendEmailAsync(user.Id, "Re: Please confirm and login", callbackUrl);

						// create role if it does not exist
						string roleName = model.UserType;
						if (!RoleManager.RoleExists(roleName))
						{
							var role = new IdentityRole(roleName);
							await RoleManager.CreateAsync(role);
						}

						// add appropriate role to new user
						await UserManager.AddToRoleAsync(user.Id, roleName);

						// add user to user table  
						//	this.AddNewUser(user.Id);

						// sign in now
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

						AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);   ///clear cookie
						return RedirectToAction("VerifyEmail", "Account", new { Email = user.Email });
						//return RedirectToAction("Info", model.UserType);
					}

					AddErrors(result);
				}
				catch (DbEntityValidationException dbEx)
				{
					foreach (var validationErrors in dbEx.EntityValidationErrors)
					{
						foreach (var validationError in validationErrors.ValidationErrors)
						{
							Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
						}
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet, Route("account/co-ordinator-register")]
		[AllowAnonymous]
		public ActionResult CoordinatorRegister()
		{
			return View();
		}

		[HttpPost, Route("account/co-ordinator-register")]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CoordinatorRegister(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = new IdentityUser { UserName = model.Email, Email = model.Email };
					var result = await UserManager.CreateAsync(user, model.Password);

					if (result.Succeeded)
					{
						// send email to ask the newly registered user to 'confirm his/her email address' 
						string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
						await UserManager.SendEmailAsync(user.Id, "Re: Please confirm and login", callbackUrl);

						// create role if it does not exist
						string roleName = model.UserType;
						if (!RoleManager.RoleExists(roleName))
						{
							var role = new IdentityRole(roleName);
							await RoleManager.CreateAsync(role);
						}

						// add appropriate role to new user
						await UserManager.AddToRoleAsync(user.Id, roleName);

						// add user to user table  
						//	this.AddNewUser(user.Id);

						// sign in now
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

						AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);   ///clear cookie
						return RedirectToAction("VerifyEmail", "Account", new { Email = user.Email });
						//return RedirectToAction("Info", model.UserType);
					}

					AddErrors(result);
				}
				catch (DbEntityValidationException dbEx)
				{
					foreach (var validationErrors in dbEx.EntityValidationErrors)
					{
						foreach (var validationError in validationErrors.ValidationErrors)
						{
							Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
						}
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet, Route("account/supervisor-register")]
		[AllowAnonymous]
		public ActionResult SupervisorRegister()
		{
			return View();
		}

		[HttpPost, Route("account/supervisor-register")]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SupervisorRegister(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = new IdentityUser { UserName = model.Email, Email = model.Email };
					var result = await UserManager.CreateAsync(user, model.Password);

					if (result.Succeeded)
					{
						// send email to ask the newly registered user to 'confirm his/her email address' 
						string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
						await UserManager.SendEmailAsync(user.Id, "Re: Please confirm and login", callbackUrl);

						// create role if it does not exist
						string roleName = model.UserType;
						if (!RoleManager.RoleExists(roleName))
						{
							var role = new IdentityRole(roleName);
							await RoleManager.CreateAsync(role);
						}

						// add appropriate role to new user
						await UserManager.AddToRoleAsync(user.Id, roleName);

						// add user to user table  
						//	this.AddNewUser(user.Id);

						// sign in now
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

						AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);   ///clear cookie
						return RedirectToAction("VerifyEmail", "Account", new { Email = user.Email });
						//return RedirectToAction("Info", model.UserType);
					}

					AddErrors(result);
				}
				catch (DbEntityValidationException dbEx)
				{
					foreach (var validationErrors in dbEx.EntityValidationErrors)
					{
						foreach (var validationError in validationErrors.ValidationErrors)
						{
							Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
						}
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet, Route("account/co-supervisor-register")]
		[AllowAnonymous]
		public ActionResult CoSupervisorRegister()
		{
			return View();
		}

		[HttpPost, Route("account/co-supervisor-register")]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CoSupervisorRegister(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = new IdentityUser { UserName = model.Email, Email = model.Email };
					var result = await UserManager.CreateAsync(user, model.Password);

					if (result.Succeeded)
					{
						// send email to ask the newly registered user to 'confirm his/her email address' 
						string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
						await UserManager.SendEmailAsync(user.Id, "Re: Please confirm and login", callbackUrl);

						// create role if it does not exist
						string roleName = model.UserType;
						if (!RoleManager.RoleExists(roleName))
						{
							var role = new IdentityRole(roleName);
							await RoleManager.CreateAsync(role);
						}

						// add appropriate role to new user
						await UserManager.AddToRoleAsync(user.Id, roleName);

						// add user to user table  
						//	this.AddNewUser(user.Id);

						// sign in now
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

						AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);   ///clear cookie
						return RedirectToAction("VerifyEmail", "Account", new { Email = user.Email });
						//return RedirectToAction("Info", model.UserType);
					}

					AddErrors(result);
				}
				catch (DbEntityValidationException dbEx)
				{
					foreach (var validationErrors in dbEx.EntityValidationErrors)
					{
						foreach (var validationError in validationErrors.ValidationErrors)
						{
							Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
						}
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}


		[HttpGet, Route("account/external-supervisor-register")]
		[AllowAnonymous]
		public ActionResult ExternalSupervisorRegister()
		{
			return View();
		}

		[HttpPost, Route("account/external-supervisor-register")]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalSupervisorRegister(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = new IdentityUser { UserName = model.Email, Email = model.Email };
					var result = await UserManager.CreateAsync(user, model.Password);

					if (result.Succeeded)
					{
						// send email to ask the newly registered user to 'confirm his/her email address' 
						string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
						await UserManager.SendEmailAsync(user.Id, "Re: Please confirm and login", callbackUrl);

						// create role if it does not exist
						string roleName = model.UserType;
						if (!RoleManager.RoleExists(roleName))
						{
							var role = new IdentityRole(roleName);
							await RoleManager.CreateAsync(role);
						}

						// add appropriate role to new user
						await UserManager.AddToRoleAsync(user.Id, roleName);

						// add user to user table  
						//	this.AddNewUser(user.Id);

						// sign in now
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

						AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);   ///clear cookie
						return RedirectToAction("VerifyEmail", "Account", new { Email = user.Email });
						//return RedirectToAction("Info", model.UserType);
					}

					AddErrors(result);
				}
				catch (DbEntityValidationException dbEx)
				{
					foreach (var validationErrors in dbEx.EntityValidationErrors)
					{
						foreach (var validationError in validationErrors.ValidationErrors)
						{
							Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
						}
					}
				}
			}
			return View(model);
		}

		[AllowAnonymous]
		public ActionResult VerifyEmail(string Email)
		{
			ViewBag.Email = Email;
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResendEmail(string email)
		{
			ViewBag.Email = email;
			// Resend an email with this link
			return View();
		}

		[AllowAnonymous]
		public async Task<ActionResult> ResendConfirmEmail(string email)
		{
			var user = UserManager.FindByName(email);

			var loggedInUserId = user.Id;

			// Resend an email with this link
			string code = await UserManager.GenerateEmailConfirmationTokenAsync(loggedInUserId);
			var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = loggedInUserId, code = code }, protocol: Request.Url.Scheme);
			await UserManager.SendEmailAsync(loggedInUserId, "Re: Please confirm and login", callbackUrl);

			return RedirectToAction("VerifyEmail", "Account", new { Email = email });
			//  return RedirectToAction("Status", "Manage", new { Message = ManageController.ManageMessageId.ResendConfirmEmailSuccess });
		}

		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				// don't reveal that the user does not exist or is not confirmed
				return View("ConfirmEmailConfirmation");
			}

			//add below for email confirmation
			IdentityResult result;
			try
			{
				result = await UserManager.ConfirmEmailAsync(userId, code);
			}
			catch (InvalidOperationException ioe)
			{
				var msg = ioe.Message; // TODO: need to log this somewhere later

				// don't reveal that the user does not exist or is not confirmed
				return View("ConfirmEmailConfirmation");
			}

			if (result.Succeeded)
			{
				// add user to user table  
				AddNewUser(userId);

				return RedirectToAction("ConfirmEmailConfirmation", "Account");
			}

			// If we got this far, something failed.
			return View("Error");
		}

		[AllowAnonymous]
		public ActionResult ConfirmEmailConfirmation()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				// send an email with this link
				string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				await UserManager.SendEmailAsync(user.Id, "Re: Reset Password", callbackUrl);

				return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPassword(string code, string userId)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}

			var aspNetUser = _userManager.FindById(userId);
			if (aspNetUser == null || !(_userManager.IsEmailConfirmed(userId)))
			{
				// don't reveal that the user does not exist or is not confirmed
				return View("ForgotPasswordConfirmation");
			}

			ViewBag.Email = aspNetUser.Email;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}

			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}

			AddErrors(result);
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		#endregion

		#region Action - Unused 2FA / social login stuff

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await _signInManager.HasBeenVerifiedAsync())
			{
				return View("Error");
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					{
						// string userName = User.Identity.Name;
						//  string userId = User.Identity.GetUserId();
						//add data to client or careWorker table
						//  await UserManager.SendEmailAsync(userId, "User:"+userName, "register success");

						return RedirectToLocal(model.ReturnUrl);
					}
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Manage");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new IdentityUser { UserName = model.Email, Email = model.Email };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		#endregion

		#region Helpers

		//add a new user in user table
		private async void AddNewUser(string userId)
		{
			var aspNetUser = await _userManager.FindByIdAsync(userId);
			string userType = GetUserType(aspNetUser);
			switch (userType)   //maybe change later 
			{
				case "Student":
					_userService.CreateStudent(aspNetUser);
					break;
				case "Supervisor":
					_userService.CreateSupervisor(aspNetUser);
					break;
				case "CoSupervisor":
					_userService.CreateCoSupervisor(aspNetUser);
					break;
				case "ExternalSupervisor":
					_userService.CreateExternalSupervisor(aspNetUser);
					break;
				case "Coordinator":
					_userService.CreateCoordinator(aspNetUser);
					break;
			}
		}

		private string GetUserType(IdentityUser aspNetUser)
		{
			var roleId = aspNetUser.Roles.Single().RoleId;
			string userType = _roleManager.FindById(roleId).Name.ToString();
			return userType;
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
											: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		#endregion

	}
}