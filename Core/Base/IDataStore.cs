using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core;
public interface IDataStore<T>
{
    Task<T> GetAsync(int id);
    Task<IEnumerable<T>> GetAsync(PagingInfo? paging = null);
    Task<IEnumerable<T>> GetAsync(string propertyName, object value, PagingInfo? paging = null);
    Task<IEnumerable<T>> SearchAsync(string query, PagingInfo? paging = null, params string [] propertiesToSearch);
    Task<bool> DeleteAsync(int id);
    Task<T> AddOrUpdateAsync(T item);
    Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items);
    Task HandleException(Exception ex);
}