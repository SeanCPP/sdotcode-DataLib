using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core.DataStores;

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
        var response = (await client.GetFromJsonAsync<IEnumerable<T>>(
            $"{controllerName}/Find/Id/{id}?page=0&pageSize=1") ?? new List<T>())
            .FirstOrDefault();
        return response!;
    }

    public async Task<IEnumerable<T>> GetAsync(string propertyName, object value, PagingInfo? paging = null)
    {
        paging ??= new();
        var uri = $"{controllerName}/Find/{propertyName}/{value}?page={paging.Page}&pageSize={paging.PageSize}";
        var response = await client.GetFromJsonAsync<IEnumerable<T>>(uri);
        return response ?? new List<T>();
    }

    public async Task<IEnumerable<T>> GetAsync(PagingInfo? paging = null)
    {
        paging ??= new();
        var response = await client.GetFromJsonAsync<IEnumerable<T>>($"{controllerName}?page={paging.Page}&pageSize={paging.PageSize}");
        return response ?? new List<T>();
    }

    public async Task<IEnumerable<T>> SearchAsync(Dictionary<string, string> searchQueries, PagingInfo? paging = null)
    {
        paging ??= new();
        var sb = new StringBuilder();
        foreach (var propName in searchQueries.Keys)
        {
            sb.Append($"{propName}={searchQueries[propName]}&");
        }

        var uri = $"{controllerName}/Search?page={paging.Page}&pageSize={paging.PageSize}&{sb}";
        return await client.GetFromJsonAsync<IEnumerable<T>>(uri) ?? new List<T>();
    }

    public Task HandleException(Exception ex)
    {
        // TODO: add logging here
        Console.WriteLine(ex.Message);
        return Task.CompletedTask;
    }

}
