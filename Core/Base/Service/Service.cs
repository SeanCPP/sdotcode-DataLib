using System.Linq.Expressions;
using System.Reflection;

namespace sdotcode.DataLib.Core;

public class Service<T> : ErrorProne where T : IStoredItem, new()
{
    protected IDataStore<T> DataStore { get; init; }

    public Service(IDataStore<T> dataStore)
    {
        DataStore = dataStore;
    }

    #region Extension API

    /// <summary>
    /// On Get Single. Don't invoke this method directly, the system will invoke it. (Use Get(int) instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<T> OnGet(int id) => DataStore.GetAsync(id);

    /// <summary>
    /// On Get Multiple. Don't invoke this method directly, the system will invoke it. (Use Get instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnGet(PagingInfo paging) => DataStore.GetAsync(paging);

    /// <summary>
    /// On Get Multiple. Don't invoke this method directly, the system will invoke it. (Use Get instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnGet(string propertyName, object value, PagingInfo paging)
        => DataStore.GetAsync(propertyName, value, paging);

    /// <summary>
    /// On Search. Don't invoke this method directly, the system will invoke it. (Use Get instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnSearch(Dictionary<string, string> searchQueries, PagingInfo? paging = null)
        => DataStore.SearchAsync(searchQueries, paging);

    /// <summary>
    /// On Update Single. Don't invoke this method directly, the system will invoke it. (Use Update instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<T> OnUpsert(T entity) => DataStore.AddOrUpdateAsync(entity);

    /// <summary>
    /// On Update Multiple. Don't invoke this method directly, the system will invoke it. (Use Update instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnUpsert(IEnumerable<T> items) => DataStore.AddOrUpdateAsync(items);


    /// <summary>
    /// On Delete Single. Don't invoke this method directly, the system will invoke it. (Use Delete instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<bool> OnDelete(int id) => DataStore.DeleteAsync(id);

    /// <summary>
    /// Invoked when an Exception occurs within an operation. If you override this method, it will still invoke the IDataSource implementation's
    /// HandleException. Don't invoke this method directly, the system will invoke it.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnException(Exception ex) { return Task.CompletedTask; }

    #endregion

    #region Public API

    public Task<T> GetAsync(int id) => Try(() => OnGet(id));
    
    public Task<IEnumerable<T>> GetAsync(PagingInfo? pagingOptions = null) 
        => Try<IEnumerable<T>, List<T>>(() => OnGet(pagingOptions ?? new()));

    public Task<IEnumerable<T>> GetAsync(string propertyName, object value, PagingInfo? pagingOptions = null) 
        => Try<IEnumerable<T>, List<T>>(() => OnGet(propertyName, value, pagingOptions ?? new()));

    public Task<IEnumerable<T>> GetAsync(Expression<Func<T, object?>> propertyExpr, object value, PagingInfo? pagingOptions = null)
    {
        return Try<IEnumerable<T>, List<T>>(() =>
        {
            if (propertyExpr is null || propertyExpr.Body is null)
            {
                throw new ArgumentException("Invalid property expression provided.");
            }

            if (propertyExpr.Body is not MemberExpression body)
            {
                UnaryExpression ubody = (UnaryExpression)propertyExpr.Body;
                body = ubody?.Operand as MemberExpression ?? throw new ArgumentException("Invalid property expression provided."); ;
            }

            return OnGet(body!.Member.Name, value, pagingOptions ?? new());
        });
    }

    public Task<IEnumerable<T>> SearchAsync(string query, PagingInfo? pagingOptions = null, params string[] propertiesToSearch)
    {
        return Try<IEnumerable<T>, List<T>>(() => 
        {
            var actualSearchParams = new Dictionary<string, string>();
            foreach (var property in propertiesToSearch)
            {
                var prop = typeof(T).GetProperty(property);
                if (prop is null)
                {
                    continue;
                }

                var attribute = prop.GetCustomAttribute(typeof(SearchableAttribute), inherit: false);
                if (attribute is not null)
                {
                    actualSearchParams[property] = query;
                }
            }
            return OnSearch(actualSearchParams, pagingOptions ?? new());
        });
    }
    
    public Task<IEnumerable<T>> SearchAsync(Dictionary<string, string> propertySearches, PagingInfo? pagingOptions = null)
    {
        return Try<IEnumerable<T>, List<T>>(() =>
        {
            var filteredSearches = new Dictionary<string, string>();

            if(propertySearches is null)
            {
                return OnGet(pagingOptions!);
            }

            foreach (var property in propertySearches.Keys)
            {
                var prop = typeof(T).GetProperty(property);
                if (prop is null)
                {
                    continue;
                }
                var attribute = prop.GetCustomAttribute(typeof(SearchableAttribute), inherit: false);
                if (attribute is null)
                {
                    continue;
                }

                filteredSearches[property] = propertySearches[property] ?? string.Empty;
            }
            return OnSearch(filteredSearches, pagingOptions ?? new());
        });
    }
    
    public Task<IEnumerable<T>> SearchAsync(string query, PagingInfo? pagingOptions = null, params Expression<Func<T, object>>[] propertiesToSearch)
    {
        return Try<IEnumerable<T>, List<T>>(() => 
        { 
            var propertyStrings = new List<string>();

            foreach (var propertyExpr in propertiesToSearch)
            {
                if (propertyExpr is null || propertyExpr.Body is null)
                {
                    throw new ArgumentException("Invalid property expression(s) provided.");
                }

                if (propertyExpr.Body is not MemberExpression body)
                {
                    UnaryExpression ubody = (UnaryExpression)propertyExpr.Body;
                    body = ubody?.Operand as MemberExpression ?? throw new ArgumentException("Invalid property expression(s) provided."); ;
                }

                propertyStrings.Add(body!.Member.Name);
            }
            return SearchAsync(query, pagingOptions ?? new(), propertyStrings.ToArray());
        });
    }

    public Task<IEnumerable<T>> SearchAsync<SearchType>(SearchType searchModel, PagingInfo? pagingOptions = null)
        where SearchType : new()
    {
        return Try<IEnumerable<T>, List<T>>(() =>
        {
            var searchType = typeof(SearchType);
            var entityType = typeof(T);
            var propertyStrings = new Dictionary<string, string>();

            var searchProperties = searchType.GetProperties();
            var properties = entityType.GetProperties();

            foreach (var property in searchProperties)
            {
                var propertyToSearch = properties
                    .FirstOrDefault(p => p.Name == property.Name 
                        && p.PropertyType == property.PropertyType);
                if(propertyToSearch is null)
                {
                    continue;
                }

                var value = property.GetValue(searchModel)?.ToString() ?? string.Empty;
                if (value is not null)
                {
                    propertyStrings[property.Name] = value;
                }
                
            }
            return SearchAsync(propertyStrings, pagingOptions ?? new());
        });
    }

    public Task<T> UpsertAsync(T entity) => Try(() => OnUpsert(entity));
    
    public Task<IEnumerable<T>> UpsertAsync(IEnumerable<T> items) => Try<IEnumerable<T>, List<T>>(() => OnUpsert(items));
    
    public Task<bool> DeleteAsync(int id) => Try(() => OnDelete(id));

    #endregion

    #region Private Methods

    // This will be invoked when an error is thrown inside a call to Try()
    protected sealed override Task HandleException(Exception ex)
    {
        DataStore.HandleException(ex);
        return OnException(ex);
    }

    #endregion
}