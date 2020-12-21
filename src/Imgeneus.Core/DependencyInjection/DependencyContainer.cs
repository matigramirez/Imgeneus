using Imgeneus.Core.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Imgeneus.Core.DependencyInjection
{
    public sealed class DependencyContainer : Singleton<DependencyContainer>, IDisposable
    {
        private IServiceCollection services;
        private ServiceProvider serviceProvider;
        public IConfiguration configuration;

        /// <summary>
        /// Gets the number of registered services.
        /// </summary>
        public int Count => this.services.Count;

        /// <summary>
        /// Creates a new <see cref="DependencyContainer"/> instance.
        /// </summary>
        public DependencyContainer()
        {
            this.services = new ServiceCollection();

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", optional: true)
#else
                .AddJsonFile($"appsettings.Release.json", optional: true)
#endif
                .Build();

            services.AddSingleton(configuration);
        }

        /// <summary>
        /// Sets the dependency container's service collection.
        /// </summary>
        /// <param name="serviceCollection">Existing service collection.</param>
        /// <returns>This object instance.</returns>
        public DependencyContainer SetServiceCollection(IServiceCollection serviceCollection)
        {
            this.services = serviceCollection;
            return this;
        }

        /// <summary>
        /// Build the service provider of the dependency container.
        /// </summary>
        /// <returns>The service provider.</returns>
        public IServiceProvider BuildServiceProvider()
        {
            if (this.serviceProvider == null && this.services != null)
            {
                this.serviceProvider = this.services.BuildServiceProvider();
            }

            return this.serviceProvider;
        }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        /// <returns>A collection of services.</returns>
        public IServiceCollection GetServiceCollection() => this.services;

        /// <summary>
        /// Register a new service.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="serviceLifetime">The service life time.</param>
        public void Register(Type implementationType, Type serviceType, ServiceLifetime serviceLifetime)
        {
            if (this.services == null)
            {
                throw new InvalidOperationException("Cannot register dependency. ServiceCollection has not been initialized. Please call the Initialize() method.");
            }

            Func<Type, Type, IServiceCollection> addServiceMethod;

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    addServiceMethod = this.services.AddSingleton;
                    break;

                case ServiceLifetime.Scoped:
                    addServiceMethod = this.services.AddScoped;
                    break;
                default:
                    addServiceMethod = this.services.AddTransient;
                    break;
            }

            addServiceMethod(implementationType, serviceType);
        }

        /// <summary>
        /// Register a new service.
        /// </summary>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="serviceLifetime">The service life time.</param>
        public void Register<TImplementation, TService>(ServiceLifetime serviceLifetime)
            => this.Register(typeof(TImplementation), typeof(TService), serviceLifetime);

        /// <summary>
        /// Resolves a dependency.
        /// </summary>
        /// <param name="type">Dependency type to resolve.</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            if (this.serviceProvider == null)
            {
                throw new InvalidOperationException("Cannot resolve dependency. Service Provider has not been initialized. Please call the BuildServiceProvider() method.");
            }

            return this.serviceProvider.GetService(type);
        }

        /// <summary>
        /// Resolve a dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class => this.Resolve(typeof(T)) as T;

        /// <summary>
        /// Configure the <see cref="DependencyContainer"/>.
        /// </summary>
        /// <param name="serviceBuilder"></param>
        public void Configure(Action<IServiceCollection> serviceBuilder) => serviceBuilder(this.services);


        /// <summary>
        /// Dispose the dependency container's resources.
        /// </summary>
        public void Dispose()
        {
            this.services.Clear();

            if (this.serviceProvider != null)
            {
                this.serviceProvider.Dispose();
                this.serviceProvider = null;
            }
        }
    }
}
