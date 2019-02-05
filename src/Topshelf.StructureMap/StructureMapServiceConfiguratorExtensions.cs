using System;
using Topshelf.ServiceConfigurators;

namespace Topshelf.StructureMap
{
    public static class StructureMapServiceConfiguratorExtensions
    {
        public static Func<T> GetFactory<T>() where T : class => () => StructureMapBuilderConfigurator.Container.GetInstance<T>();

        public static ServiceConfigurator<T> ConstructUsingStructureMap<T>(this ServiceConfigurator<T> configurator) where T : class {

			var hasInterfaces = typeof(T).GetInterfaces()?.Length > 0;

			if (hasInterfaces && IsAssignable<ServiceControl, T>()) {
                configurator.WhenStarted((service, control) => ((ServiceControl)service).Start(control));
                configurator.WhenStopped((service, control) => ((ServiceControl)service).Stop(control));
            }

			if (hasInterfaces && IsAssignable<ServiceSessionChange, T>()) {
				configurator.WhenSessionChanged((service, control, args) => ((ServiceSessionChange)service).SessionChange(control, args));
			}

			configurator.ConstructUsing(GetFactory<T>());
            return configurator;
        }

		static bool IsAssignable<TInterface, TService>()
		{
			return typeof(TInterface).IsAssignableFrom(typeof(TService));
		}
    }
}