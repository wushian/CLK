using CLK.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLK.Web.Mvc.Samples.No001.Models
{
    public enum GenderType
    {
        Unknown, // 未知
        Male,    // 男
        Female,  // 女         
    }

    public sealed partial class GenderTypeConverter : ModelConverter
    {
        // Methods
        public override string Serialize(object value)
        {
            // Default
            string result = null;

            // Source
            var source = value as GenderType?;
            if (source.HasValue == false) return result;
            
            // Convert
            switch (source.Value)
            {
                case GenderType.Male: result = "1"; break;
                case GenderType.Female: result = "2"; break;
                case GenderType.Unknown: result = "3"; break;
            }

            // Return
            return result;
        }

        public override object Deserialize(string value)
        {
            // Default
            object result = null;

            // Source
            var source = value;
            if (string.IsNullOrEmpty(source) == true) return result;

            // Convert
            switch (source)
            {
                case "1": result = GenderType.Male; break;
                case "2": result = GenderType.Female; break;
                case "3": result = GenderType.Unknown; break;
            }

            // Return
            return result;
        }
    }

    public sealed partial class GenderTypeConverter
    {
        // Singleton
        private static GenderTypeConverter _instance = null;

        public static GenderTypeConverter Current
        {
            get
            {
                // Create
                if (_instance == null)
                {
                    _instance = new GenderTypeConverter();
                }

                // Return
                return _instance;
            }
        }
    }

    public sealed class GenderTypeAttribute : ModelConverterAttribute
    {
        // Methods
        protected override ModelConverter GetModelConverter()
        {
            // Return
            return GenderTypeConverter.Current;
        }
    }
}