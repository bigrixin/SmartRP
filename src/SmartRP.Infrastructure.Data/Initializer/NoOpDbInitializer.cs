using System.Data.Entity;

namespace SmartRP.Infrastructure.Data
{
	public class NoOpDbInitializer : IDatabaseInitializer<SmartRPDbContext>
	{

		public void InitializeDatabase(SmartRPDbContext db)
		{
			// do nothing, assume db already exists
		}

	}
}