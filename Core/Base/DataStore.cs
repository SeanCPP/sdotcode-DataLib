using System.Reflection;

namespace sdotcode.DataLib.Core;

public abstract class DataStore<T>
{
    protected string GetTableName(Type type)
    {
        var attribute = type.GetCustomAttribute(typeof(TableNameAttribute));
        if(attribute is not null)
        {
            return ((TableNameAttribute)attribute).Name;
        }
        return $"{type.Name}s";
    }
}

