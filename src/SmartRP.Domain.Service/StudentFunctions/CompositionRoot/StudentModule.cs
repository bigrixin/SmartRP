using Autofac;

namespace SmartRP.Domain.Service
{
	public class StudentModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			// register StudentService
			builder
				.RegisterType<StudentService>()
				.As<IStudentService>();
		}
	}
}