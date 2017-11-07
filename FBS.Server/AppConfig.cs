using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Server
{
    internal class AppConfig
    {
        public int? Port { get; set; }

        public AppConfig()
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            foreach (string key in settings.Keys)
            {
                var pi = this.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi != null)
                {
                    var propertyType = pi.PropertyType;
                    if (propertyType.IsGenericType
                        && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //if it's null, just set the value from the reserved word null, and return
                        if (settings.Get(key) == null)
                        {
                            pi.SetValue(this, null, null);
                            return;
                        }

                        //Get the underlying type property instead of the nullable generic
                        propertyType = new NullableConverter(propertyType).UnderlyingType;
                    }

                    var convertedValue = Convert.ChangeType(settings.Get(key), propertyType);
                    pi.SetValue(this, convertedValue, null);
                }
            }
        }
    }
}
