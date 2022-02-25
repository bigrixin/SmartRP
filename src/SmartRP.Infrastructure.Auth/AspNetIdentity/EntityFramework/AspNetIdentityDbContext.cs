using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;


namespace SmartRP.Infrastructure.Auth
{
	public class AspNetIdentityDbContext : IdentityDbContext<IdentityUser>
	{
		public AspNetIdentityDbContext() : base("SmartRPDbContext")
		{
		}

		public AspNetIdentityDbContext(IDatabaseInitializer<AspNetIdentityDbContext> initializer) : base("SmartRPDbContext")
		{
			if (initializer == null)
				throw new ArgumentNullException("initializer");

			// Set the database intializer which is run once during application start
			// This seeds the database with admin user credentials and admin role
			Database.SetInitializer(initializer);
		}
	}
}