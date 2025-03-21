using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace RazorPage.Helpers
{
    public class Utils
    {
        private static Utils _instance;
        private static readonly object _lock = new object();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Utils();
                    }
                    return _instance;
                }
            }
        }

        public string ExportToJson<T>(IEnumerable<T> data, List<string> selectedColumns = null)
        {
            var filteredData = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                var dict = new Dictionary<string, object>();
                var properties = item.GetType().GetProperties();

                foreach (var property in properties)
                {
                    if (selectedColumns == null || selectedColumns.Contains(property.Name))
                    {
                        dict[property.Name] = property.GetValue(item);
                    }
                }

                filteredData.Add(dict);
            }

            return JsonConvert.SerializeObject(filteredData, Formatting.Indented);
        }
    }
}
