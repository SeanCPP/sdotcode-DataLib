using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.Repository.DataStores;

public class InMemoryDataStore<T> : DataStore<T>, IDataStore<T> where T : IStoredItem, new()
{
    private readonly List<T> db = new();

    public Task<IEnumerable<T>> GetAsync(string propertyName, object value)
    {
        var results = db.Where(item => ComparePropertyWithValue(item, propertyName, value));
        return Task.FromResult(results)!;
    }

    public Task<T> AddOrUpdateAsync(T item)
    {
        if (db.Contains(item))
        {
            var toUpdate = db.FirstOrDefault(x => x.Id == item.Id);
            if(toUpdate != null)
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
        foreach(var item in items)
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

    public Task<IEnumerable<T>> GetAsync(int page=0, int pageSize=15)
    {
        return Task.FromResult(
            db
            .Skip(page*pageSize)
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

