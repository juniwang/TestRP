using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Entity
{
    public static class EntityPropertyConverter
    {
        public static void Serialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> results)
        {
            foreach (var property in entity.GetType().GetProperties())
            {
                var attributedProperty = (ConvertableEntityPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(ConvertableEntityPropertyAttribute));
                if (attributedProperty != null)
                {
                    var propertyValue = entity.GetType().GetProperty(property.Name)?.GetValue(entity);
                    results.Add(property.Name, new EntityProperty(attributedProperty.Serialize(propertyValue)));
                }
            }
        }

        public static void DeSerialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            foreach (var property in entity.GetType().GetProperties())
            {
                var attributedProperty = (ConvertableEntityPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(ConvertableEntityPropertyAttribute));
                if (attributedProperty != null && properties.ContainsKey(property.Name))
                {
                    Type resultType = property.PropertyType;
                    if (attributedProperty.ConvertToType != null)
                    {
                        resultType = attributedProperty.ConvertToType;
                    }

                    var objectValue = attributedProperty.Deserialize(properties[property.Name].StringValue, resultType);
                    entity.GetType().GetProperty(property.Name)?.SetValue(entity, objectValue);
                }
            }
        }
    }
}
