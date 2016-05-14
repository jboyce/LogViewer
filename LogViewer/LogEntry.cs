using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace LogViewer
{
    public class LogEntry : DynamicObject, IFieldAccessable, IDictionary<string, object>
    {
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _fields.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _fields[binder.Name] = value;
            return true;
        }

        public void AddField(string name, object value)
        {
            _fields[name] = value;
        }

        public IEnumerable<string> GetFieldNames()
        {
            return _fields.Keys;
        }

        public ICollection<string> Keys
        {
            get
            {
                return _fields.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return _fields.Values;
            }
        }

        public int Count
        {
            get
            {
                return _fields.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[string key]
        {
            get
            {
                return _fields[key];
            }

            set
            {
                _fields[key] = value;
            }
        }        

        public bool ContainsKey(string key)
        {
            return _fields.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _fields.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _fields.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _fields.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            (_fields as ICollection<object>).Add(item);
        }

        public void Clear()
        {
            _fields.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return (_fields as ICollection<object>).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            (_fields as ICollection<object>).CopyTo(array.Select(a => a.Value).ToArray(), arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return (_fields as ICollection<object>).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _fields.GetEnumerator();
        }
    }

    public interface IFieldAccessable
    {
        void AddField(string name, object value);
        IEnumerable<string> GetFieldNames();
    }
}
