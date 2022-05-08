using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core.DataStores
{
    public class InMemoryReadOnlyDataStore<T> : DataStore<T>, IReadOnlyDataStore<T> 
        where T : IStoredItem, new()
    {
        private readonly List<T> db = new();

        public Task<IEnumerable<T>> GetAsync(string propertyName, object value)
        {
            var results = db.Where(item => ComparePropertyWithValue(item, propertyName, value));
            return Task.FromResult(results)!;
        }

        public Task<T> GetAsync(int id)
        {
            return Task.FromResult(db.FirstOrDefault(item => item.Id == id))!;
        }

        public Task<IEnumerable<T>> GetAsync(int page = 0, int pageSize = 15)
        {
            return Task.FromResult(
                db
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToArray()
                .AsEnumerable());
        }

        public Task HandleException(Exception ex)
        {
            Console.WriteLine("Handled in the datastore");
            return Task.CompletedTask;
        }
    }
}
