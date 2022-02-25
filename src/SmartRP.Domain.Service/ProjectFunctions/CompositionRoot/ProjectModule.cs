using Autofac;

namespace SmartRP.Domain.Service
{
	public class ProjectModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			// register ProjectService
			builder
				.RegisterType<ProjectService>()
				.As<IProjectService>();
		}
	}
}