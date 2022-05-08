using System.Reflection;

namespace sdotcode.DataLib.Core;

public abstract class Service<T> : ErrorProne, IGetService<T>, IUpsertService<T>, IDeleteService<T> where T : IStoredItem, new()
{
    protected IDataStore<T> DataStore { get; init; }

    public Service(IDataStore<T> dataStore)
    {
        this.DataStore = dataStore;
    }

    /// <summary>
    /// On Get Multiple. Don't invoke this method directly, the system will invoke it. (Use Get() instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnGet(int page=0, int pageSize=Defaults.PageSize) => DataStore.GetAsync(page, pageSize);

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
    /// On Update Single. Don't invoke this method directly, the system will invoke it. (Use Update(entity) instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<T> OnAddOrUpdate(T entity) => DataStore.AddOrUpdateAsync(entity);

    /// <summary>
    /// On Update Single. Don't invoke this method directly, the system will invoke it. (Use Update(entity) instead)
    /// </summary>
    /// <returns></returns>
    protected virtual Task<IEnumerable<T>> OnAddOrUpdate(IEnumerable<T> items) => DataStore.AddOrUpdateAsync(items);


    /// <summary>
    /// On Delete Single. Don't invoke this method directly, the system will invoke it. (Use Delete(predicate) instead)
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

    public Task<IEnumerable<T>> GetAsync(int page = 0, int pageSize = Defaults.PageSize)
    {
        return Try<IEnumerable<T>, List<T>>(async () => await OnGet(page, pageSize));
    }
    public Task<T> GetAsync(int id)
    {
        return Try(async () => await OnGet(id));
    }
    public Task<IEnumerable<T>> GetAsync(string propertyName, object value)
    {
        return Try<IEnumerable<T>, List<T>>(async () => await OnGet(propertyName, value));
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