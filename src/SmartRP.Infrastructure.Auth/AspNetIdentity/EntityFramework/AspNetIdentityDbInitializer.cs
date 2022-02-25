using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace SmartRP.Infrastructure.Auth
{
	// This is useful if you do not want to tear down the database each time you run the application.
	// public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
	// This example shows you how to create a new database if the Model changes
	public class AspNetIdentityDbInitializer : CreateDatabaseIfNotExists<AspNetIdentityDbContext>
	{

		protected override void Seed(AspNetIdentityDbContext context)
		{
		//	InitializeIdentityForEF(context);  // it is for auto migration
			base.Seed(context);
		}

		private void InitializeIdentityForEF(AspNetIdentityDbContext db)
		{
			var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(db));
			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

			const string name = "mysmartrp+admin@gmail.com";
			const string password = "Admin@123456";
			const string roleName = "Admin";

			//Create Role Admin if it does not exist
			var role = roleManager.FindByName(roleName);
			if (role == null)
			{
				role = new IdentityRole(roleName);
				var roleresult = roleManager.Create(role);
			}

			var user = userManager.FindByName(name);
			if (user == null)
			{
				user = new IdentityUser { UserName = name, Email = name };
				var result = userManager.Create(user, password);
				result = userManager.SetLockoutEnabled(user.Id, false);
			}

			// Add user admin to Role Admin if not already added
			var rolesForUser = userManager.GetRoles(user.Id);
			if (!rolesForUser.Contains(role.Name))
			{
				var result = userManager.AddToRole(user.Id, role.Name);
			}
		}

	}
}