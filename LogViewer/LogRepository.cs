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
                { "Timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromDays(2)).ToString() },
                { "Message", "Some informational message.  this is a really reallly really long value. fieldvalue1. this is a really reallly really long value. fieldvalue1. this is a really reallly really long value. fieldvalue1. this is a really reallly really long value" },
                { "CustomField1", "custom field1 text" }
            };

            var entry2 = new Dictionary<string, string>
            {
                { "Level", "Warning" },
                { "Timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToString() },
                { "Message", "Some warning message" },
                { "CustomField2", "custom field2 text" }
            };

            var entry3 = new Dictionary<string, string>
            {
                { "Level", "Error" },
                { "Timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)).ToString() },
                { "Message", "Some error message" },
                { "CustomField3", "custom field3 text" }
            };

            var entry4 = new Dictionary<string, string>
            {
                { "Level", "Fatal" },
                { "Timestamp", DateTime.UtcNow.ToString() },
                { "Message", "I ate broccoli" },
                { "CustomField4", "custom field4 text" }
            };

            LogRepository.AddEntry(entry1);
            LogRepository.AddEntry(entry2);
            LogRepository.AddEntry(entry3);
            LogRepository.AddEntry(entry4);
        }

        private static List<dynamic> _allEntries = new List<dynamic>();

        public static void AddEntry(Dictionary<string, string> entry)
        {
            dynamic expandoEntry = new ExpandoObject();
            var entryAsDictionary = expandoEntry as IDictionary<string, object>;
            foreach (var keyPair in entry)
            {
                object value = keyPair.Value;
                if (keyPair.Key == "Timestamp")
                    value = Convert.ToDateTime(value);
                entryAsDictionary.Add(keyPair.Key, value);
            }

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
