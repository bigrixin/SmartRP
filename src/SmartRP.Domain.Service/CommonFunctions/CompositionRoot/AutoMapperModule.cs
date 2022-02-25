using Autofac;
using AutoMapper;
using System.Collections.Generic;
using System.Reflection;

namespace SmartRP.Domain.Service
{
	public class AutoMapperModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//register all profile classes in the calling assembly
			builder.RegisterAssemblyTypes(typeof(AutoMapperModule).Assembly).As<Profile>();

			builder.Register(context => new MapperConfiguration(cfg =>
			{
				foreach (var profile in context.Resolve<IEnumerable<Profile>>())
				{
					cfg.AddProfile(profile);
				}
			})).AsSelf().SingleInstance();

			builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
					.As<IMapper>()
					.InstancePerLifetimeScope();


			// register Profile types via assembly scan
			/* 	builder
						.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
						.AssignableTo(typeof(Profile))
						.As<Profile>();

					builder
						.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
						.AsClosedTypesOf(typeof(IValueResolver<,,>))
						.AsImplementedInterfaces();

					// register MapperConfiguration
					builder
						.Register(c => new MapperConfiguration(cfg =>
						{
							foreach (var profile in c.Resolve<IEnumerable<Profile>>())
								cfg.AddProfile(profile);
						}))
						.AsSelf()
						.SingleInstance();

					// register IMapper service
					builder
						.Register(c =>
						{
							// http://stackoverflow.com/questions/40293597/configuring-automapper-to-fulfil-itypeconverter-constructor-dependecies-with/40306029
							var _ctx = c.Resolve<IComponentContext>();
							return c.Resolve<MapperConfiguration>().CreateMapper(_ctx.Resolve);
						})
						.As<IMapper>()
						.InstancePerLifetimeScope();
			 */
		}
	}
}
