using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace jp.co.mirabo.Application.Data.Sync
{
    public class Entity
    {
        public string type = default(string);
        public string id = default(string);
        public Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

        public Entity(string id, string type)
        {
            this.id = id;
            this.type = type;
        }
    }

    public class Attribute
    {
        public string dataType;
        public string dataValue;
    }
}