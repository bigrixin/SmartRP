
namespace SmartRP.Infrastructure.Auth.Migrations
{
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using System.Data.Entity.Migrations;

	internal sealed class Configuration : DbMigrationsConfiguration<AspNetIdentityDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			ContextKey = "AspNetIdentityDbContext";
		}

		protected override void Seed(AspNetIdentityDbContext context)
		{
			//  This method will be called after migrating to the latest version.
 
		}
 
	}
}
