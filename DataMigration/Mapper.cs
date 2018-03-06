using System;
using System.Collections.Generic;

namespace DataMigration
{
    public class Mapper
    {
        private readonly Dictionary<string, Guid> _dictionary = new Dictionary<string, Guid>();

        public bool Exists(string key)
        {
            return _dictionary.ContainsKey(key.ToLower());
        }

        public void Add(string key, Guid value)
        {
            _dictionary.Add(key.ToLower(), value);
        }

        public Guid GetId(string key)
        {            
            return _dictionary[key.ToLower()];
        }

        public IEnumerable<Guid> GetValues() => _dictionary.Values;

    }
}