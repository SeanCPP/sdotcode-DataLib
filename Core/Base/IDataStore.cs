using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core;
public interface IDataStore<T>
{
    Task<T> GetAsync(int id);
    Task<IEnumerable<T>> GetAsync(int page = 0, int pageSize = Defaults.PageSize);
    Task<IEnumerable<T>> GetAsync(string propertyName, object value);
    Task<bool> DeleteAsync(int id);
    Task<T> AddOrUpdateAsync(T item);
    Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items);
    Task HandleException(Exception ex);
}