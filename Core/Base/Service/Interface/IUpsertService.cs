namespace sdotcode.DataLib.Core
{
    public interface IUpsertService<T>
    {
        Task<T> AddOrUpdateAsync(T entity);
        Task<IEnumerable<T>> AddOrUpdateAsync(IEnumerable<T> items);
    }
}