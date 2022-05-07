using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sdotcode.Repository.DataStores;

/// <summary>
/// A generic datastore that uses an HttpClient to map to a basic CRUD (RESTful) API controller. The API Controller MUST
/// adhere to the "ExtendedControllerBase" <see langword="abstract"/> <see langword="class"/> for this to work.
/// </summary>
/// <typeparam name="T"></typeparam>
public class HttpClientDataStore<T> : DataStore<T>, IDataStore<T> where T : IStoredItem, new()
{
    private readonly HttpClient client;

    private readonly string controllerName = string.Empty;

    public HttpClientDataStore(HttpClient client)
    {
        this.client = client;
        controllerName = GetTableName(typeof(T));
    }
    public async Task<T> AddOrUpdateAsync(T item)
    {
        var response = await client.PutAsJsonAsync($"{controllerName}/", new List<T> { item });
        if (response.IsSuccessStatusCode)
        {
            return item;
        }
        else
        {
            return default!;
        }
    }

    public async Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items)
    {
        var response = await client.PutAsJsonAsync($"{controllerName}/", items);
        if (response.IsSuccessStatusCode)
        {
            return items;
        }
        else
        {
            return default!;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await client.DeleteAsync($"{controllerName}?id={id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<T> GetAsync(int id)
    {
        var response = await client.GetFromJsonAsync<T>($"{controllerName}/Id/{id}");
        return response!;
    }
    public async Task<IEnumerable<T>> GetAsync(string propertyName, object value)
    {
        var uri = $"{controllerName}/find/{propertyName}/{value}";
        var response = await client.GetFromJsonAsync<IEnumerable<T>>(uri);
        return response ?? new List<T>();
    }

    public async Task<IEnumerable<T>> GetAsync(int page=0, int pageSize = 15)
    {
        var response = await client.GetFromJsonAsync<IEnumerable<T>>($"{controllerName}?page={page}&pageSize={pageSize}");
        return response ?? new List<T>();
    }

    public Task HandleException(Exception ex)
    {
        // TODO: add logging here
        Console.WriteLine(ex.Message);
        return Task.CompletedTask;
    }

}
