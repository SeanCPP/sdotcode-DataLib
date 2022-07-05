using Microsoft.Extensions.DependencyInjection;
using sdotcode.DataLib.Core.DataStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core
{
    public enum ServiceLifetime
    {
        Scoped,
        Singleton,
        Transient
    }
    public static partial class ServiceExtensions
    {
        private static IServiceCollection Register<TEntity, TDataStoreImpl>(this IServiceCollection services, ServiceLifetime lifetime)
            where TEntity : IStoredItem, new()
            where TDataStoreImpl : class, IDataStore<TEntity>
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IDataStore<TEntity>, TDataStoreImpl>();
                    services.AddScoped<Service<TEntity>>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IDataStore<TEntity>, TDataStoreImpl>();
                    services.AddSingleton<Service<TEntity>>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<IDataStore<TEntity>, TDataStoreImpl>();
                    services.AddTransient<Service<TEntity>>();
                    break;
            }
            return services;
        }

        private static IServiceCollection Register<TEntity, TDataStoreImpl, TServiceImpl>(this IServiceCollection services, ServiceLifetime lifetime)
            where TEntity : IStoredItem, new()
            where TDataStoreImpl : class, IDataStore<TEntity>
            where TServiceImpl : Service<TEntity>
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped<IDataStore<TEntity>, TDataStoreImpl>();
                    services.AddScoped<Service<TEntity>, TServiceImpl>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IDataStore<TEntity>, TDataStoreImpl>();
                    services.AddSingleton<Service<TEntity>, TServiceImpl>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<IDataStore<TEntity>, TDataStoreImpl>();
                    services.AddTransient<Service<TEntity>, TServiceImpl>();
                    break;
            }
            return services;
        }

        private static IServiceCollection Register(this IServiceCollection services,
           Type entityType,
           Type dataStoreImplType,
           ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(IDataStore<>).MakeGenericType(entityType), dataStoreImplType.MakeGenericType(entityType));
                    services.AddScoped(typeof(Service<>).MakeGenericType(entityType));
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(IDataStore<>).MakeGenericType(entityType), dataStoreImplType.MakeGenericType(entityType));
                    services.AddSingleton(typeof(Service<>).MakeGenericType(entityType));
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(IDataStore<>).MakeGenericType(entityType), dataStoreImplType.MakeGenericType(entityType));
                    services.AddTransient(typeof(Service<>).MakeGenericType(entityType));
                    break;
            }
            return services;
        }

        private static IServiceCollection Register(this IServiceCollection services,
            Type entityType,
            Type dataStoreImplType,
            Type serviceImplType,
            ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(IDataStore<>).MakeGenericType(entityType), dataStoreImplType.MakeGenericType(entityType));
                    services.AddScoped(typeof(Service<>).MakeGenericType(entityType), serviceImplType);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(IDataStore<>).MakeGenericType(entityType), dataStoreImplType.MakeGenericType(entityType));
                    services.AddSingleton(typeof(Service<>).MakeGenericType(entityType), serviceImplType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(IDataStore<>).MakeGenericType(entityType), dataStoreImplType.MakeGenericType(entityType));
                    services.AddTransient(typeof(Service<>).MakeGenericType(entityType), serviceImplType);
                    break;
            }
            return services;
        }

        private static IEnumerable<Type> GetTypesInNamespace(this AssemblyName[] assemblies, string targetNamespace)
        {
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(Assembly.Load(assembly).GetTypes().Where(type => type.Namespace == targetNamespace));
            }
            return types;
        }

        public static IServiceCollection AddWebApiRepository<TEntity, TServiceImpl>(this IServiceCollection services)
            where TEntity : IStoredItem, new()
            where TServiceImpl : Service<TEntity>
        {
            services.Register<TEntity, HttpClientDataStore<TEntity>, TServiceImpl>(ServiceLifetime.Scoped);
            return services;
        }

        public static IServiceCollection AddWebApiRepositoryFor<TEntity>(this IServiceCollection services)
            where TEntity : IStoredItem, new()
        {
            services.Register<TEntity, HttpClientDataStore<TEntity>>(ServiceLifetime.Scoped);
            return services;
        }

        public static IServiceCollection AddWebApiRepositoryFor(this IServiceCollection services, 
            string entitiesNamespace, 
            IDictionary<Type, Type>? overrides = null)
        {
            overrides ??= new Dictionary<Type, Type>();

            var types = Assembly.GetCallingAssembly()
                .GetReferencedAssemblies()
                .GetTypesInNamespace(entitiesNamespace);

            foreach (var type in types)
            {
                if (overrides.ContainsKey(type))
                {
                    continue;
                }
                services.Register(type, typeof(HttpClientDataStore<>), ServiceLifetime.Scoped);
            }

            foreach (var entityType in overrides.Keys)
            {
                services.Register(entityType, typeof(HttpClientDataStore<>), overrides[entityType], ServiceLifetime.Scoped);
            }
            return services;
        }

        public static IServiceCollection AddInMemoryRepositoryFor<TEntity>(this IServiceCollection services, 
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TEntity : IStoredItem, new()
        {
            services.Register<TEntity, InMemoryDataStore<TEntity>>(lifetime);
            return services;
        }

        public static IServiceCollection AddInMemoryRepository<TEntity, TServiceImpl>(this IServiceCollection services, 
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TEntity : IStoredItem, new()
            where TServiceImpl : Service<TEntity>
        {
            services.Register<TEntity, InMemoryDataStore<TEntity>, TServiceImpl>(lifetime);
            return services;
        }
    }
}
