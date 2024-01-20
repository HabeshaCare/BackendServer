using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Utils
{
    public class NullObjects
    {
        public static bool IsAllNullOrEmpty(object? obj)
        {
            if (obj is null)
                return true;

            return obj.GetType().GetProperties()
                .All(x => IsNullOrEmpty(x.GetValue(obj)));
        }
        public static bool IsAnyNullOrEmpty(object obj)
        {
            if (obj is null)
                return true;

            return obj.GetType().GetProperties()
                .Any(x => IsNullOrEmpty(x.GetValue(obj)));
        }

        private static bool IsNullOrEmpty(object? value)
        {
            if (value is null)
                return true;

            var type = value.GetType();
            return type.IsValueType
                && Equals(value, Activator.CreateInstance(type));
        }
    }
}