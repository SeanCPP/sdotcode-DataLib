using System.Reflection;

namespace sdotcode.DataLib.Core;

public abstract class DataStore<T>
{
    protected (Type, object) GetPropertyValue(object item, string propertyName)
    {
        var properties = typeof(T).GetProperties();
        var prop = properties.FirstOrDefault(p => p.Name == propertyName);
        if (prop is null)
        {
            throw new ArgumentException("Invalid property name was provided.");
        }
        return (prop.PropertyType, prop!.GetValue(item)!);
    }
    protected bool ComparePropertyWithValue(object item, string propertyName, object value)
    {
        var (type, result) = GetPropertyValue(item, propertyName);
        var val1 = Convert.ChangeType(result, type);
        var val2 = Convert.ChangeType(value, type);
        return result is not null && val1.Equals(val2);
    }
    public string GetTableName(Type type)
    {
        var attribute = type.GetCustomAttribute(typeof(TableNameAttribute));
        if(attribute is not null)
        {
            return ((TableNameAttribute)attribute).Name;
        }
        return type.Name;
    }
}

