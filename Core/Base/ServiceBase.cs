using System.Reflection;

namespace sdotcode.Repository;

abstract class ServiceBase<T> where T : new()
{
    /// <summary>
    /// On Get Multiple. Don't invoke this method directly, the system will invoke it. (Use Get() instead)
    /// </summary>
    /// <returns></returns>
    protected abstract Task<IEnumerable<T>> OnGet();

    /// <summary>
    /// On Get Single. Don't invoke this method directly, the system will invoke it. (Use Get(int) instead)
    /// </summary>
    /// <returns></returns>
    protected abstract Task<T> OnGet(int id);

    /// <summary>
    /// On Update Single. Don't invoke this method directly, the system will invoke it. (Use Update(entity) instead)
    /// </summary>
    /// <returns></returns>
    protected abstract Task<T> OnUpdate(T entity);

    /// <summary>
    /// On Delete Single. Don't invoke this method directly, the system will invoke it. (Use Delete(int) instead)
    /// </summary>
    /// <returns></returns>
    protected abstract Task OnDelete(int id);

    /// <summary>
    /// On Exception Occured. Don't invoke this method directly, the system will invoke it.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnException(Exception ex) => null!;



    private bool HasAttribute(string methodName, Func<MethodInfo, bool> matchMethod, Type attributeType)
    {
        var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        var get = methods.FirstOrDefault(method => method.IsFamily && method.Name == methodName && matchMethod(method));
        if (Attribute.IsDefined(get!, attributeType))
        {
            return true;
        }
        return false;
    }

    private async Task<U> Try<U> (Func<Task<U>> getter) where U : new()
    {
        try
        {
            return await getter();
        }
        catch (Exception ex)
        {
            await OnException(ex);
            return new();
        }
    }
    private async Task<InterfaceType> Try<InterfaceType, ConcreteType>(Func<Task<InterfaceType>> getter) 
        where ConcreteType : InterfaceType, new()
    {
        try
        {
            return await getter();
        }
        catch (Exception ex)
        {
            await OnException(ex);
            return new ConcreteType();//default!;
        }
    }


    public Task<IEnumerable<T>> Get()
    {
        var currentMethod = MethodBase.GetCurrentMethod();
        if (HasAttribute(
            nameof(OnGet),
            method => StandardMatchPredicate(currentMethod!, method), 
            typeof(TryAttribute)))
        {
            return Try<IEnumerable<T>, List<T>>(async () => 
            {
                return await OnGet();
            });
        }
        else
        {
            return OnGet();
        }
    }
    public Task<T> Get(int id)
    {
        var currentMethod = MethodBase.GetCurrentMethod();
        if (HasAttribute(
            nameof(OnGet),
            method => StandardMatchPredicate(currentMethod!, method),
            typeof(TryAttribute)))
        {
            return Try(async () =>
            {
                return await OnGet(id);
            });
        }
        else
        {
            return OnGet(id);
        }
    }
    public Task<T> Update(T entity)
    {
        var currentMethod = MethodBase.GetCurrentMethod();
        if (HasAttribute(
            nameof(OnUpdate),
            method => StandardMatchPredicate(currentMethod!, method),
            typeof(TryAttribute)))
        {
            return Try(async () =>
            {
                return await OnUpdate(entity);
            });
        }
        else
        {
            return OnUpdate(entity);
        }
    }
    public Task Delete(int id)
    {
        var currentMethod = MethodBase.GetCurrentMethod();
        if (HasAttribute(
            nameof(OnDelete),
            method => StandardMatchPredicate(currentMethod!, method),
            typeof(TryAttribute)))
        {
            return Try<None>(async () =>
            {
                await OnDelete(id);
                return null!;
            });
        }
        else
        {
            return OnDelete(id);
        }
    }

    private bool StandardMatchPredicate(MethodBase mine, MethodInfo compareWith)
    {
        var myParameters = mine.GetParameters();
        bool matchesAll = true;
        if (myParameters.Length == compareWith.GetParameters().Length)
        {
            var theirparameters = compareWith.GetParameters();
            for (int i = 0; i < myParameters.Length; ++i)
            {
                if (myParameters[i].ParameterType != theirparameters[i].ParameterType)
                {
                    matchesAll = false;
                }
            }
        }
        return mine!.GetParameters().Length == compareWith.GetParameters().Length && matchesAll;
    }
}