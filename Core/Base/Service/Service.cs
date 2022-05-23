using System.Linq.Expressions;
using System.Reflection;

namespace sdotcode.DataLib.Core;

public abstract class Service<T> : ErrorProne where T : IStoredItem, new()
{
    protected IDataStore<T> DataStore { get; init; }

    public Service(IDataStore<T> dataStore)
    {
        DataStore = dataStore;
    }

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
    /// On Get Multiple. Don't invoke this method directly, the system will invoke it. (Use Get instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnSearch(string query, PagingInfo paging, params string[] propertiesToSearch) 
        => DataStore.SearchAsync(query, paging, propertiesToSearch);

    /// <summary>
    /// On Update Single. Don't invoke this method directly, the system will invoke it. (Use Update instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<T> OnAddOrUpdate(T entity) => DataStore.AddOrUpdateAsync(entity);

    /// <summary>
    /// On Update Single. Don't invoke this method directly, the system will invoke it. (Use Update instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnAddOrUpdate(IEnumerable<T> items) => DataStore.AddOrUpdateAsync(items);


    /// <summary>
    /// On Delete Single. Don't invoke this method directly, the system will invoke it. (Use Delete instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<bool> OnDelete(int id) => DataStore.DeleteAsync(id);

    /// <summary>
    /// On Exception Occured in any of the "On" methods. If you override this method, it's generally a good idea to "return base.OnException(ex)" to allow the
    /// IDataSource implementation to handle its errors.
    /// Don't invoke this method directly, the system will invoke it.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnException(Exception ex) => DataStore.HandleException(ex);


    protected sealed override Task HandleException(Exception ex)
    {
        return OnException(ex);
    }

    public Task<IEnumerable<T>> GetAsync(PagingInfo? pagingOptions = null) 
        => Try<IEnumerable<T>, List<T>>(async () => await OnGet(pagingOptions ?? new()));

    public Task<T> GetAsync(int id) => Try(async () => await OnGet(id));

    public Task<IEnumerable<T>> GetAsync(string propertyName, object value, PagingInfo? pagingOptions = null) 
        => Try<IEnumerable<T>, List<T>>(async () => await OnGet(propertyName, value, pagingOptions ?? new()));

    public Task<IEnumerable<T>> GetAsync(Expression<Func<T, object?>> propertyExpr,
        object value,
        PagingInfo? pagingOptions = null)
    {
        return Try<IEnumerable<T>, List<T>>(async () =>
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

            return await Try<IEnumerable<T>, List<T>>(async () => await OnGet(body!.Member.Name, value, pagingOptions ?? new()));
        });
    }

    public Task<IEnumerable<T>> SearchAsync(string query, PagingInfo? pagingOptions = null, params string[] propertiesToSearch)
    {
        return Try<IEnumerable<T>, List<T>>(async () => 
        {
            var actualSearchParams = new List<string>();
            foreach (var property in propertiesToSearch)
            {
                var prop = typeof(T).GetProperty(property);
                if (prop is null)
                {
                    continue;
                }

                var attribute = prop.GetCustomAttribute(typeof(SearchableAttribute));
                if (attribute is not null)
                {
                    actualSearchParams.Add(property);
                }
            }
            return await OnSearch(query, pagingOptions ?? new(), actualSearchParams.ToArray());
        });
    }

    public Task<IEnumerable<T>> SearchAsync(string query, 
        PagingInfo? pagingOptions = null, 
        params Expression<Func<T, object>>[] propertiesToSearch)
    {
        return Try<IEnumerable<T>, List<T>>(async () => 
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
            return await SearchAsync(query, pagingOptions ?? new(), propertyStrings.ToArray());
        });
    }

    public Task<T> AddOrUpdateAsync(T entity)
    {
        return Try(async () => await OnAddOrUpdate(entity));
    }
    public Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items)
    {
        return Try<IEnumerable<T>, List<T>>(async () => await OnAddOrUpdate(items));
    }
    public Task<bool> DeleteAsync(int id)
    {
        return Try(() => OnDelete(id));
    }
}