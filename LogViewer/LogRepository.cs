using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer
{
    public static class LogRepository
    {
        //remove this
        static LogRepository()
        {
            var entry1 = new Dictionary<string, string>
            {
                { "Level", "Info" },
                { "Timestamp", DateTime.UtcNow.ToString() },
                { "Message", "Some informational message" }
            };

            var entry2 = new Dictionary<string, string>
            {
                { "Level", "Error" },
                { "Timestamp", DateTime.UtcNow.ToString() },
                { "Message", "Some error message" }
            };

            LogRepository.AddEntry(entry1);
            LogRepository.AddEntry(entry2);
        }

        private static List<dynamic> _allEntries = new List<dynamic>();

        public static void AddEntry(Dictionary<string, string> entry)
        {
            dynamic expandoEntry = new ExpandoObject();
            var entryAsDictionary = expandoEntry as IDictionary<string, object>;
            foreach (var keyPair in entry)
                entryAsDictionary.Add(keyPair.Key, keyPair.Value);

            _allEntries.Add(expandoEntry);
        }

        public static IEnumerable<dynamic> GetAll()
        {
            return _allEntries;
        }

        public static IEnumerable<string> GetUniqueFieldNames()
        {
            var uniqueFields = _allEntries.Cast<IDictionary<string, object>>().SelectMany(e => e.Keys).Distinct();
            return uniqueFields;
        }

        public static string GetFieldValue(dynamic entry, string fieldName)
        {
            object value;
            if ((entry as IDictionary<string, object>).TryGetValue(fieldName, out value))
                return value.ToString();
            return null;
        }
    }
}
