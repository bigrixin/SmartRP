using SmartRP.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SmartRP.Infrastructure.Data
{
	public class CreateFromScratchDbInitializer : CreateDatabaseIfNotExists<SmartRPDbContext>
	{

		protected override void Seed(SmartRPDbContext context)
		{
		/*
			var now = DateTime.Now;

			var steve = new Coordinator(Guid.NewGuid().ToString());
			steve.Firstname = "Steven";
			steve.Lastname = "Zhai";
			steve.CreatedAt = now;
			steve.UpdatedAt = now;

			var clients = new List<Coordinator> { steve };
			clients.ForEach(m => context.Users.Add(m));

			context.SaveChanges();
			*/
		}

	}
}
