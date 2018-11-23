using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLK.Web.Mvc
{
    public sealed class ModelSerializer
    {
        // Methods
        public static string Serialize(object value = null, string prefix = null)
        {
            // Require
            if (value == null) return string.Empty;

            // Result
            var nameValueDictionary = new Dictionary<string, string>();

            // Serialize
            SerializeObject(ref nameValueDictionary, value, prefix);

            // Return
            return string.Join("&", nameValueDictionary.Select(nameValuePair => (nameValuePair.Key + "=" + Uri.EscapeDataString(nameValuePair.Value))));
        }

        private static void SerializeObject(ref Dictionary<string, string> nameValueDictionary, object value = null, string prefix = null)
        {
            // Require
            if (value == null) return;

            // Distribute
            if (value is string) { SerializeValueType(ref nameValueDictionary, value as string, prefix); return; }
            if (value is IEnumerable) { SerializeEnumerable(ref nameValueDictionary, value as IEnumerable, prefix); return; }

            if (value.GetType().IsValueType == false)
            {
                SerializeReferenceType(ref nameValueDictionary, value, prefix);
                return;
            }
            else
            {
                SerializeValueType(ref nameValueDictionary, value, prefix);
                return;
            }
        }

        private static void SerializeEnumerable(ref Dictionary<string, string> nameValueDictionary, IEnumerable valueCollection = null, string prefix = null)
        {
            // Require
            if (valueCollection == null) return;

            // Create  
            int index = 0;
            foreach (var subValue in valueCollection)
            {
                // SubPrefix
                string subPrefix = null;
                if (string.IsNullOrEmpty(prefix) == false)
                {
                    subPrefix = prefix + "[" + index.ToString() + "]";
                }
                else
                {
                    subPrefix = "[" + index.ToString() + "]";
                }

                // Index
                index++;

                // Serialize
                SerializeObject(ref nameValueDictionary, subValue, subPrefix);                
            }
        }

        private static void SerializeReferenceType(ref Dictionary<string, string> nameValueDictionary, object value = null, string prefix = null)
        {
            // Require
            if (value == null) return;

            // PropertyDescriptorArray
            var propertyDescriptorArray = PropertyDescriptorFactory.Current.GetAll(value.GetType());
            if (propertyDescriptorArray == null) throw new InvalidOperationException();

            // Create
            foreach (var propertyDescriptor in propertyDescriptorArray)
            {
                // SubPrefix
                var subPrefix = propertyDescriptor.Name;
                if (string.IsNullOrEmpty(subPrefix) == true) continue;
                if (string.IsNullOrEmpty(prefix) == false)
                {
                    subPrefix = prefix + "." + subPrefix;
                }

                // SubValue
                var subValue = propertyDescriptor.GetValue(value);
                if (subValue == null) continue;

                // Convert
                var propertyModelConverterAttribute = propertyDescriptor.Attributes.OfType<CustomModelConverterAttribute>().FirstOrDefault();
                if (propertyModelConverterAttribute != null)
                {
                    var propertyModelConverter = propertyModelConverterAttribute.GetConverter();
                    if (propertyModelConverter != null)
                    {
                        subValue = propertyModelConverter.Serialize(subValue);
                    }
                }

                // Serialize
                SerializeObject(ref nameValueDictionary, subValue, subPrefix);
            }
        }

        private static void SerializeValueType(ref Dictionary<string, string> nameValueDictionary, object value = null, string prefix = null)
        {
            // Require
            if (value == null) return;

            // Name
            var name = prefix;
            if (name == null) name = string.Empty;

            // Serialize
            if (value is DateTime) { nameValueDictionary.Add(name, ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff")); return; }
            nameValueDictionary.Add(name, value.ToString());
        }
    }
}
