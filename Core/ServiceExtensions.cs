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
    public static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiRepository<TEntity, TServiceImpl>(this IServiceCollection services)
            where TEntity : IStoredItem, new()
            where TServiceImpl : Service<TEntity>
        {
            services.AddScoped<IDataStore<TEntity>, HttpClientDataStore<TEntity>>();
            services.AddScoped<Service<TEntity>, TServiceImpl>();
            return services;
        }

        public static IServiceCollection AddWebApiRepositoryFor<TEntity>(this IServiceCollection services)
            where TEntity : IStoredItem, new()
        { 
            services.AddScoped<IDataStore<TEntity>, HttpClientDataStore<TEntity>>();
            services.AddScoped<Service<TEntity>>();
            return services;
        }

        public static IServiceCollection AddWebApiRepositoryFor(this IServiceCollection services, 
            string entitiesNamespace, 
            IDictionary<Type, Type>? overrides = null)
        {
            overrides ??= new Dictionary<Type, Type>();

            var assemblies = Assembly.GetCallingAssembly().GetReferencedAssemblies();
            foreach(var assembly in assemblies)
            {
                var types = Assembly.Load(assembly).GetTypes().Where(type => type.Namespace == entitiesNamespace);
                foreach (var type in types)
                {
                    if (overrides.ContainsKey(type))
                    {
                        continue;
                    }
                    services.AddScoped(typeof(IDataStore<>).MakeGenericType(type), typeof(HttpClientDataStore<>).MakeGenericType(type));
                    services.AddScoped(typeof(Service<>).MakeGenericType(type));
                }
            }

            foreach(var entityType in overrides.Keys)
            {
                services.AddScoped(typeof(IDataStore<>).MakeGenericType(entityType), typeof(HttpClientDataStore<>).MakeGenericType(entityType));
                services.AddScoped(typeof(Service<>).MakeGenericType(entityType), overrides[entityType]);
            }
            return services;
        }
    }
}
