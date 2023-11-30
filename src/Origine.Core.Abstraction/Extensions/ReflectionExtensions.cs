using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Origine
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<FieldInfo> GetConstFieldByValue<T>(this Type type, T val) where T : struct
        {
            var fields = type.GetRuntimeFields();
            try
            {
                return fields.Where(f =>
                {
                    var rawValue = f.GetRawConstantValue();
                    return ((T)rawValue).Equals(val);
                });
            }
            catch
            {
                return null;
            }
        }

        public static FieldInfo GetFirstConstFieldByValue<T>(this Type type, T val) where T : struct
        {
            return GetConstFieldByValue(type, val)?.First();
        }

        public static IEnumerable<Type> GetSubclassesFromAssembly(this Type type, Assembly asm = null)
        {
            asm = asm ?? type.Assembly;
            return asm.GetTypes().Where(t => t.IsSubclassOf(type));
        }

        public static IEnumerable<Type> GetAllSubclassesFromAssembly(this Type type, Assembly asm = null)
        {
            asm = asm ?? type.Assembly;
            return asm.GetTypes().Where(t => type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttributes>(this Type type) where TAttributes : Attribute
        {
            return type.GetRuntimeMethods().Where(m => m.GetCustomAttribute<TAttributes>() != null);
        }
    }
}
