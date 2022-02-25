using SmartRP.Domain;
using SmartRP.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

public static class ControllerExtensions
{

	#region Extensions

	public static User GetLoggedInUser(this Controller controller)
	{
		if (!controller.HttpContext.Request.IsAuthenticated)
			return null;

		var entities = DependencyResolver.Current.GetService<IReadEntities>();
		var loginIdentityId = controller.HttpContext.User.Identity.GetUserId();
		User user = null;

		if (controller.HttpContext.User.IsInRole("Coordinator"))
			user = entities.Single<Coordinator>(u => u.LoginIdentityID == loginIdentityId);
		else if (controller.HttpContext.User.IsInRole("Supervisor"))
			user = entities.Single<Supervisor>(u => u.LoginIdentityID == loginIdentityId);
		else if (controller.HttpContext.User.IsInRole("CoSupervisor"))
			user = entities.Single<CoSupervisor>(u => u.LoginIdentityID == loginIdentityId);
		else if (controller.HttpContext.User.IsInRole("ExternalSupervisor"))
			user = entities.Single<ExternalSupervisor>(u => u.LoginIdentityID == loginIdentityId);
		else if (controller.HttpContext.User.IsInRole("Student"))
			user = entities.Single<Student>(u => u.LoginIdentityID == loginIdentityId);

		return user;
	}

	#endregion

}
