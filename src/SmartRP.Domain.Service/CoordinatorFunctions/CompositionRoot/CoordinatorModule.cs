using Autofac;

namespace SmartRP.Domain.Service
{
    public class CoordinatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // register CoordinatorService
            builder
                .RegisterType<CoordinatorService>()
                .As<ICoordinatorService>();
        }
    }
}