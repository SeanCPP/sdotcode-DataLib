using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdotcode.DataLib.Core
{
    public static class ReflectionHelpers
    {
        public static object GetPropertyValue<T>(object item, string propertyName)
        {
            var properties = typeof(T).GetProperties();
            var prop = properties.FirstOrDefault(p => p.Name == propertyName);
            if (prop is null)
            {
                throw new ArgumentException("Invalid property name was provided.");
            }
            return prop!.GetValue(item)!;
        }

        public static (Type, object) GetPropertyTypeAndValue<T>(object item, string propertyName)
        {
            var properties = typeof(T).GetProperties();
            var prop = properties.FirstOrDefault(p => p.Name == propertyName);
            if (prop is null)
            {
                throw new ArgumentException("Invalid property name was provided.");
            }
            return (prop.PropertyType, prop!.GetValue(item)!);
        }
        public static bool ComparePropertyValue<T>(object item, string propertyName, object value)
        {
            var (type, result) = GetPropertyTypeAndValue<T>(item, propertyName);
            var val1 = Convert.ChangeType(result, type);
            var val2 = Convert.ChangeType(value, type);
            return result is not null && val1.Equals(val2);
        }
    }
}
