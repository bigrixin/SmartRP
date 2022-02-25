using Autofac;
using System.Configuration;
using System.Data.Entity;


namespace SmartRP.Infrastructure.Data
{
	public class EntityFrameworkModule : Autofac.Module
	{

		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			bool shouldRecreateDb;
#if DEBUG
			bool.TryParse(ConfigurationManager.AppSettings["recreateDb"], out shouldRecreateDb);
#else
			shouldRecreateDb = false;
#endif

			// register initializer
			if (shouldRecreateDb)
				builder.RegisterType<CreateFromScratchDbInitializer>()
				.As<IDatabaseInitializer<SmartRPDbContext>>();

			// register database context
			builder
				.RegisterType<SmartRPDbContext>()
			  .AsImplementedInterfaces()
				.AsSelf()
				
				//.As<IReadEntities>()
				//.As<IWriteEntities>()
				.InstancePerLifetimeScope();

			// register data objects
			builder
				.RegisterType<DbData>();
		}
	}
}