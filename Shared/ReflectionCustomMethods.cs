using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared
{
    public class ReflectionCustomMethods
    {

        //source https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
        /// <summary>
        /// Used to dynamicly retreives auxiliary points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public T GetPropertyValue<T>(object obj, string propName)
        {
            object? result = obj.GetType()?.GetProperty(propName)?.GetValue(obj, null);

            if(result == null)
            {
                return default;
            }
            return (T)result;
        }

    }
}
