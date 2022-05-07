using Microsoft.Extensions.DependencyInjection;
using sdotcode.Repository;
using sdotcode.Repository.DataStores;
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
            services.AddSingleton<IDataStore<T>, HttpClientDataStore<T>>();
            services.AddSingleton<Service<T>, ServiceType>();
            return services;
        }
    }
}
