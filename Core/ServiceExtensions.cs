using Microsoft.Extensions.DependencyInjection;
using sdotcode.DataLib.Core.DataStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHttpClientDataStore<T, ServiceType>(this IServiceCollection services)
            where T : IStoredItem, new()
            where ServiceType : Service<T>
        {
            services.AddScoped<IDataStore<T>, HttpClientDataStore<T>>();
            services.AddScoped<Service<T>, ServiceType>();
            return services;
        }

        public static IServiceCollection AddHttpClientReadonlyDataStore<T, ServiceType>(this IServiceCollection services)
            where T : IStoredItem, new()
            where ServiceType : ReadOnlyService<T>
        {
            services.AddScoped<IReadOnlyDataStore<T>, HttpClientReadOnlyDataStore<T>>();
            services.AddScoped<ReadOnlyService<T>, ServiceType>();
            return services;
        }
    }
}
