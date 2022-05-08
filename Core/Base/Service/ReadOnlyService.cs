using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core
{
    public class ReadOnlyService<T> : ErrorProne, IGetService<T> 
        where T : IStoredItem, new()
    {
        protected IDataStore<T> DataStore { get; init; }
        public ReadOnlyService(IDataStore<T> dataStore)
        {
            this.DataStore = dataStore;
        }


        /// <summary>
        /// On Get Multiple. Don't invoke this method directly, the system will invoke it. (Use Get() instead)
        /// </summary>
        /// <returns></returns>
        protected virtual Task<IEnumerable<T>> OnGet(int page = 0, int pageSize = Defaults.PageSize) => DataStore.GetAsync(page, pageSize);

        /// <summary>
        /// On Get Single. Don't invoke this method directly, the system will invoke it. (Use Get(int) instead)
        /// </summary>
        /// <returns></returns>
        protected virtual Task<T> OnGet(int id) => DataStore.GetAsync(id);

        /// <summary>
        /// On Get Single. Don't invoke this method directly, the system will invoke it. (Use Get(string, object) instead)
        /// </summary>
        /// <returns></returns>
        protected virtual Task<IEnumerable<T>> OnGet(string propertyName, object value) => DataStore.GetAsync(propertyName, value);
        
        /// <summary>
        /// On Exception Occured in any of the "On" methods. If you override this method, it's generally a good idea to "return base.OnException(ex)" to allow the
        /// IDataSource implementation to handle its errors.
        /// Don't invoke this method directly, the system will invoke it.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnException(Exception ex) => DataStore.HandleException(ex);
        public Task<IEnumerable<T>> GetAsync(int page = 0, int pageSize = 10) 
            => Try<IEnumerable<T>, List<T>>(() => OnGet(page, pageSize));

        
        public Task<T> GetAsync(int id) => Try(() => OnGet(id));

        public Task<IEnumerable<T>> GetAsync(string propertyName, object value) 
            => Try<IEnumerable<T>, List<T>>(() => OnGet(propertyName, value));

        protected override Task HandleException(Exception ex) => OnException(ex);
    }
}
