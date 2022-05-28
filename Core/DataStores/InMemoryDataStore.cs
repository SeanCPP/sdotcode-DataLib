using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core.DataStores;

public class InMemoryDataStore<T> : DataStore<T>, IDataStore<T> where T : IStoredItem, new()
{
    private readonly List<T> db = new();
    private readonly PropertyInfo[] entityProperties;

    public InMemoryDataStore()
    {
        entityProperties = typeof(T).GetProperties();
    }

    public Task<IEnumerable<T>> GetAsync(string propertyName, object value, PagingInfo? paging = null)
    {
        paging ??= new();

        var results = db
            .Where(item => ReflectionHelpers.ComparePropertyValue<T>(item, propertyName, value))
            .Skip(paging.Page * paging.PageSize)
            .Take(paging.PageSize);
        return Task.FromResult(results)!;
    }

    public Task<T> AddOrUpdateAsync(T item)
    {
        if (db.Contains(item))
        {
            var toUpdate = db.FirstOrDefault(x => x.Id == item.Id);
            if (toUpdate != null)
            {
                toUpdate = item;
                return Task.FromResult(toUpdate);
            }
        }
        else
        {
            db.Add(item);
        }
        return Task.FromResult(item);
    }

    public Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            if (db.Contains(item))
            {
                var toUpdate = db.FirstOrDefault(x => x.Id == item.Id);
                if (toUpdate != null)
                {
                    toUpdate = item;
                }
            }
            else
            {
                db.Add(item);
            }
        }
        return Task.FromResult(items);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var item = db.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            db.Remove(item);
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(false);
        }
    }

    public Task<T> GetAsync(int id)
    {
        return Task.FromResult(db.FirstOrDefault(item => item.Id == id))!;
    }

    public Task<IEnumerable<T>> GetAsync(PagingInfo? paging = null)
    {
        paging ??= new();

        return Task.FromResult(
            db
            .Skip(paging.Page * paging.PageSize)
            .Take(paging.PageSize));
    }

    public Task<IEnumerable<T>> SearchAsync(Dictionary<string, string> searchQueries, PagingInfo? paging = null)
    {
        paging ??= new(); 

        var result = db.Where(item =>
        {
            bool match = true;
            foreach (var searchPropertyName in searchQueries.Keys)
            {
                var entityProperty = entityProperties.FirstOrDefault(prop => prop.Name == searchPropertyName);

                var entityPropertyValue = ReflectionHelpers.GetPropertyValue<T>(item, entityProperty!.Name)
                              .ToString()?
                              .ToLower()
                              ?? string.Empty;

                var searchPropertyValue = searchQueries[searchPropertyName];

                if (!entityPropertyValue.Contains(searchPropertyValue.ToLower()))
                {
                    match = false;
                }
            }
            return match;
        });
       
        return Task.FromResult(
            result
                .Skip(paging.Page * paging.PageSize)
                .Take(paging.PageSize) 
            ?? new List<T>());
    }

    public Task HandleException(Exception ex)
    {
        Console.WriteLine("Handled in the datastore");
        return Task.CompletedTask;
    }

}

