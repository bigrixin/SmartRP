
namespace SmartRP.Infrastructure.Data.Migrations
{
	using Infrastructure.Auth;
	using System.Data.Entity.Migrations;

	internal sealed class Configuration : DbMigrationsConfiguration<SmartRPDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(SmartRPDbContext context)
		{
			//  This method will be called after migrating to the latest version.
		}
	}
}
