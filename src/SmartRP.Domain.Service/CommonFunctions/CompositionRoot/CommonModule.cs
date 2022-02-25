using Autofac;

namespace SmartRP.Domain.Service
{
	public class CommonModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			// register UserService
			builder
				.RegisterType<UserService>()
				.As<IUserService>();

			//Register CommonService
			builder
			.RegisterType<CommonService>()
			.As<ICommonService>();

			//Register UploadService
			builder
			.RegisterType<UploadService>()
			.As<IUploadService>();
		}
	}
}
