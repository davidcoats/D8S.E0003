using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using R5T.T0132;


namespace D8S.E0003
{
	[FunctionalityMarker]
	public partial interface IOperations : IFunctionalityMarker
	{
		public void TryDisposeOfServiceProvider()
        {
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			var serviceProviderAsIServiceProvider = serviceProvider.GetService<IServiceProvider>();

			// Result:
			// Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceProviderEngineScope: service provider type.
			Console.WriteLine($"{serviceProviderAsIServiceProvider.GetType().FullName}: service provider type.");

			// Error CS0122  'ServiceProviderEngineScope' is inaccessible due to its protection level
			//Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceProviderEngineScope serviceProvider;

			// Result: null, since the actual service provider type is hidden internally within the Microsoft.Extensions.DependencyInjection assembly.
			var serviceProviderAsServiceProvider = serviceProviderAsIServiceProvider as ServiceProvider;

            // Fails: null reference exception. So the service provider is safe from services!
            //serviceProviderAsServiceProvider.Dispose();

            // Well still see if the service provider works after disposal.
            serviceProvider.Dispose();

			// Now see if the service provider still works.
			// Fails: System.ObjectDisposedException: 'Cannot access a disposed object. Object name: 'IServiceProvider'.'
			var service = serviceProvider.GetService<IService>();
		}

		/// <summary>
		/// Try to get a <see cref="ServiceProvider"/> instance from the service provider.
		/// Result: fails.
		/// </summary>
		/// <remarks>
		/// <para>
		/// While you can request the service provider provide itself as an <see cref="IServiceProvider"/>, you cannot request the service provider provide itself as its actual type, <see cref="ServiceProvider>"/>.
		/// </para>
		/// <para>
		/// This makes sense, because in addition to the <see cref="IServiceProvider"/> interface, <see cref="ServiceProvider"/> implements <see cref="IDisposable"/>.
		/// The service provider is disposable because it takes responsibility for managing the lifetime of the service instances it provides, and services themselves might be disposable.
		/// What this means is that if a service could request the service provider as its <see cref="IDisposable"/>-implementing type, the service could malevolently call the dispose method and all heck would break lose!
		/// </para>
		/// </remarks>
		public void TryGetServiceProvider()
		{
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			// Fails, results in: System.InvalidOperationException: 'No service for type 'Microsoft.Extensions.DependencyInjection.ServiceProvider' has been registered.'
			var serviceProviderFromServiceProvider = serviceProvider.GetRequiredService<ServiceProvider>();
		}

		/// <summary>
		/// Try to get an <see cref="IServiceProvider"/> instance from the service provider.
		/// Result: succeeds.
		/// </summary>
		/// <remarks>
		/// <para>
		/// It is useful to get the service provider itself from a service provider.
		/// This allows code to decide what service types to request while the code is running, instead of the usual way of specifying required services when writing the code (run-time vs. compile-time).
		/// </para>
		/// <para>
		/// However, some people consider this a "code smell" and call it the "service locator pattern".
		/// It can be hard to determine if you have any gaps in your tree service dependencies; are you sure you have added ServiceZ when ServiceA depends on ServiceB, and ServiceB on ServiceC, and so on?
		/// At least when all service dependencies are declared in source code (for example in service implementation constructors) you can directly see what dependencies are required.
		/// However, if services are dynamically requested from a service provider, you are forced to reason through all of your program logic just to understand what service dependencies are required.
		/// </para>
		/// <para>
		/// Still, it's a useful technique to be aware of, and very interesting that the Microsoft service provider implementation can provide itself.
		/// </para>
		/// </remarks>
		public void TryGetIServiceProvider()
		{
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			// Succeeds.
			var serviceProviderFromServiceProvider = serviceProvider.GetRequiredService<IServiceProvider>();
		}

		/// <summary>
		/// Try to get an <see cref="ServiceCollection"/> out of the service provider.
		/// Result: fails.
		/// </summary>
		/// <remarks>
		/// <para>
		/// It makes sense that for all the reasons you can't get an <see cref="IServiceCollection"/>, you shouldn't be able to get it's implementation type either.
		/// See <see cref="TryGetIServiceCollection"/>.
		/// </para>
		/// </remarks>
		public void TryGetServiceCollection()
		{
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			// Fails, results in: System.InvalidOperationException: 'No service for type 'Microsoft.Extensions.DependencyInjection.ServiceCollection' has been registered.'
			var services = serviceProvider.GetRequiredService<ServiceCollection>();
		}

		/// <summary>
		/// Try to get an <see cref="IServiceCollection"/> out of the service provider.
		/// Result: fails.
		/// </summary>
		/// <remarks>
		/// <para>
		/// It would be useful if you could get the service collection a service provider was built from.
		/// That way you could services to analyze the list of available services. Within a service, you could request the collection of services used to build the service provider that provided the service, and iterate over the full set of services.
		/// Even if you just listed the full set of services to a file, that would be useful as a snapshot of services used in a program.
		/// </para>
		/// <para>
		/// It makes sense that you cannot get the service collection used to build the service provider:
		/// <list type="bullet">
		/// <item>
		/// A service provider is built from a set of services at one specific point in time.
		/// If you had access to a mutable service collection through the service provider, you might think you could change the set of available services after the service provider has been built, adding or removing services.
		/// For transient services this might be ok, but for singleton services this would be a real problem!
		/// It's better to maintain the separation between adding and removing services and then using those services that is provided at the point in time when the service provider is built from a specific set of services.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// But what if you explicitly *want* to have the services collection available?
		/// <list type="bullet">
		/// <item>
		/// You *can* add the service collection instance to the service collection itself! If you treat <see cref="IServiceCollection"/> just like any other service, and add the service collection instance to the service collection, then the service provider will be able to provide a service collection.
		/// But, an empty service provider cannot by itself provide its service collection.
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		public void TryGetIServiceCollection()
        {
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			// Fails, results in: System.InvalidOperationException: 'No service for type 'Microsoft.Extensions.DependencyInjection.IServiceCollection' has been registered.'
			var services = serviceProvider.GetRequiredService<IServiceCollection>();
        }

		/// <summary>
		/// Try to get a required service.
		/// Result: fails.
		/// </summary>
		public void TryGetRequiredNonExistentService()
        {
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			// Fails, results in: System.InvalidOperationException: 'No service for type 'D8S.E0003.IService' has been registered.'
			var nonExistentService = serviceProvider.GetRequiredService<IService>();
		}

		/// <summary>
		/// Try to get an enumerable of instances for a service that has not been registered.
		/// (It's an empty service provider, so no services have been registered.)
		/// Result: the returned enumerable is an empty array.
		/// </summary>
		/// <remarks>
		/// Yes, you can request an enumerable of services! You can register multiple implementations for the same definition, and when you request an enumerable of the service definition, you will get instances of each registered implementation in an enumerable.
		/// Note that if there are multiple registered implementations, if you request a single instance, you will get the implementation that was registered last.
		/// </remarks>
		public void TryGetEnumerableOfNonExistentService()
		{
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

            var nonExistentServices = serviceProvider.GetServices<IService>();
            // Note, this is the same as:
            //var nonExistentServices2 = serviceProvider.GetService(typeof(System.Collections.Generic.IEnumerable<IService>));
            //var nonExistentServices2 = serviceProvider.GetService(typeof(IService[]));
            //var nonExistentServices2 = serviceProvider.GetService(typeof(System.Collections.Generic.List<IService>));
            // The service provider has special logic for IEnumerable, which it does not have for Array or List.

            var isNull = nonExistentServices is null;

            var nonExistentServicesRepresentation = isNull
                ? "<null>"
                : nonExistentServices.ToString()
                ;

            // Result:
            // D8S.E0003.IService[]: IService instances
            // The returned enumerable for a non-existent service is not null.
            Console.WriteLine($"{nonExistentServicesRepresentation}: {nameof(IService)} instances");

            if (!isNull)
            {
                var nonExistenceServicesCount = nonExistentServices.Count();

                // Result:
                // Non existent services (Count: 0):
                //	Type: D8S.E0003.IService[]
                // The returned enumerable instance for a non-existent service is an empty array.
                Console.WriteLine($"Non existent services (Count: {nonExistenceServicesCount}):\n\tType: {nonExistentServices.GetType().FullName}");
            }
        }

		/// <summary>
		/// Try to get an instance of service that has not been registered.
		/// (It's an empty service provider, so no services have been registered.)
		/// Result: the returned instance is null.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If you request an instance of a service that has not been registered, the service provider returns null.
		/// </para>
		/// <para>
		/// Note that it's recommended to have services be reference (not value) types.
		/// The fundamental service provider method <see cref="IServiceProvider.GetService(Type)"/> returns an <see cref="object"/>.
		/// So if a service is a value type, it will be boxed.
		/// This is true even if the generically typed <see cref="ServiceProviderServiceExtensions.GetService{T}(IServiceProvider)"/> method is used since it's implementation merely casts the returned object to the desired value type.
		/// </para>
		/// </remarks>
		public void TryGetNonExistentService()
        {
			using var serviceProvider = Instances.ServicesOperator.GetEmptyServiceProvider();

			var nonExistentService = serviceProvider.GetService<IService>();

			var isNull = nonExistentService is null;

			var nonExistentServiceRepresentation = isNull
				? "<null>"
				: nonExistentService.ToString()
				;

			// Result:
			// <null>: INonExistentService instance
			// The instance returned for a non-existent service is null.
			Console.WriteLine($"{nonExistentServiceRepresentation}: {nameof(IService)} instance");
		}
	}
}