using Autofac;

namespace SmartRP.Domain.Service
{
	public class SupervisorModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			// register SupervisorService
			builder
				.RegisterType<SupervisorService>()
				.As<ISupervisorService>();
		}
	}
}