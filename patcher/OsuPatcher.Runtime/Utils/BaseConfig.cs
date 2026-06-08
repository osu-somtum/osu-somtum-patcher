using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OsuPatcher.Runtime.Utils
{
    internal abstract class BaseConfig
    {
        internal void _saveConfig(StreamWriter writer)
        {
            PropertyInfo[] fields = GetFields();
            foreach (var propertyInfo in fields)
            {
                object value = propertyInfo.GetValue(this);
                if (value != null)
                {
                    if (value is string str)
                        if (str.Length == 0) continue;
                    
                    writer.WriteLine($"{propertyInfo.Name}={value}");
                }
            }
        }

        internal void _loadConfig(StreamReader reader)
        {
            var fields = GetFields().ToDictionary(p => p.Name, p => p);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // split each line to a property name and value
                // really configmanager in a nutshell
                string[] parts = line.Split(new char[] { '=' }, 2);
                if (parts.Length == 2 && fields.TryGetValue(parts[0], out var propertyInfo))
                {
                    var typ = propertyInfo.PropertyType;

                    try
                    {
                        object value = typ.IsEnum
                            ? Enum.Parse(typ, parts[1])
                            : Convert.ChangeType(parts[1], typ);

                        propertyInfo.SetValue(this, value);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private PropertyInfo[] GetFields() =>
            GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }
}
