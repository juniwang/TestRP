using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Storage.Entity
{
    /// <summary>
    /// An Attribute to enable us write complex objects into Azure table
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConvertableEntityPropertyAttribute : Attribute
    {
        public Type ConvertToType;

        public ConvertableEntityPropertyAttribute()
        {

        }
        public ConvertableEntityPropertyAttribute(Type convertToType)
        {
            ConvertToType = convertToType;
        }

        public virtual string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public virtual object Deserialize(string value, Type resultType)
        {
            return JsonConvert.DeserializeObject(value, resultType);
        }
    }
}
