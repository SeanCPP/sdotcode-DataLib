namespace sdotcode.DataLib.Core
{
    public interface IDeleteService<T>
    {
        Task<bool> DeleteAsync(int id);
    }
}