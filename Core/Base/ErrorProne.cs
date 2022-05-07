namespace sdotcode.Repository;

public abstract class ErrorProne
{
    protected abstract Task HandleException(Exception ex);

    protected async Task<U> Try<U>(Func<Task<U>> getter) where U : new()
    {
        try
        {
            return await getter();
        }
        catch (Exception ex)
        {
            await HandleException(ex);
            return new();
        }
    }
    protected async Task<InterfaceType> Try<InterfaceType, ConcreteType>(Func<Task<InterfaceType>> getter)
        where ConcreteType : InterfaceType, new()
    {
        try
        {
            return await getter();
        }
        catch (Exception ex)
        {
            await HandleException(ex);
            return new ConcreteType();
        }
    }
}
