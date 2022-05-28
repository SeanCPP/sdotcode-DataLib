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
        public static IServiceCollection AddWebApiRepository<TEntity, TServiceImpl>(this IServiceCollection services)
            where TEntity : IStoredItem, new()
            where TServiceImpl : Service<TEntity>
        {
            services.AddScoped<IDataStore<TEntity>, HttpClientDataStore<TEntity>>();
            services.AddScoped<Service<TEntity>, TServiceImpl>();
            return services;
        }
    }
}
