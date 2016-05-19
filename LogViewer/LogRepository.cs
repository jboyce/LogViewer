using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace LogViewer
{
    public static class LogRepository
    {
        private static readonly object _lock = new object();

        //static LogRepository()
        //{
        //    var entry1 = new Dictionary<string, string>
        //    {
        //        { "Level", "Info" },
        //        { "Timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromDays(2)).ToString() },
        //        { "Message", "Some informational message.  this is a really reallly really long value. fieldvalue1. this is a really reallly really long value. fieldvalue1. this is a really reallly really long value. fieldvalue1. this is a really reallly really long value" },
        //        { "CustomField1", "custom field1 text" }
        //    };

        //    var entry2 = new Dictionary<string, string>
        //    {
        //        { "Level", "Warn" },
        //        { "Timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToString() },
        //        { "Message", "Some warning message" },
        //        { "CustomField2", "custom field2 text" }
        //    };

        //    var entry3 = new Dictionary<string, string>
        //    {
        //        { "Level", "Error" },
        //        { "Timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)).ToString() },
        //        { "Message", "Some error message" },
        //        { "CustomField3", "custom field3 text" }
        //    };

        //    var entry4 = new Dictionary<string, string>
        //    {
        //        { "Level", "Fatal" },
        //        { "Timestamp", DateTime.UtcNow.ToString() },
        //        { "Message", "I ate broccoli" },
        //        { "CustomField4", "custom field4 text" }
        //    };

        //    LogRepository.AddEntry(entry1);
        //    LogRepository.AddEntry(entry2);
        //    LogRepository.AddEntry(entry3);
        //    LogRepository.AddEntry(entry4);
        //}

        private static List<dynamic> _allEntries = new List<dynamic>();

        public static void AddEntry(Dictionary<string, string> entry)
        {
            dynamic logEntry = new LogEntry();
            foreach (var keyPair in entry)
            {
                if ((keyPair.Value == string.Empty || keyPair.Value == null) && keyPair.Key != "Message")
                    continue;

                object value = keyPair.Value;
                if (keyPair.Key == "Timestamp")
                    value = Convert.ToDateTime(value);
                else if (keyPair.Key == "Level")
                    value = Enum.Parse(typeof(LogLevel), keyPair.Value, true);
                (logEntry as IFieldAccessable).AddField(keyPair.Key, value);
            }

            lock(_lock)
            {
                _allEntries.Add(logEntry);
            }
        }

        public static IEnumerable<dynamic> GetAll()
        {
            lock (_lock)
            {
                return _allEntries.ToList();
            }
        }

        public static IEnumerable<string> GetUniqueFieldNames()
        {
            lock (_lock)
            {
                var uniqueFields = _allEntries.Cast<IFieldAccessable>().SelectMany(e => e.GetFieldNames()).Distinct();
                return uniqueFields;
            }
        }

        public static string GetFieldValue(dynamic entry, string fieldName)
        {
            object value;
            if ((entry as IDictionary<string, object>).TryGetValue(fieldName, out value))
                return value.ToString();
            return null;
        }

        public static void Clear()
        {
            lock(_lock)
            {
                _allEntries.Clear();
            }
        }
    }
}
