namespace sdotcode.DataLib.Core
{
    public interface IGetService<T>
    {
        public Task<IEnumerable<T>> GetAsync(int page = 0, int pageSize = Defaults.PageSize);
        public Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetAsync(string propertyName, object value);
    }
}