// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;
using static jp.co.mirabo.Application.RoomManagement.RoomConfig;
using SyncEntity = jp.co.mirabo.Application.Data.Sync.Entity;
using SyncAttribute = jp.co.mirabo.Application.Data.Sync.Attribute;
using jp.co.mirabo.Application.RoomManagement;
using UnityEngine;

namespace SyncRoom.Schemas {
	public partial class Entity : Schema {
		[Type(0, "string")]
		public string type = default(string);

		[Type(1, "string")]
		public string id = default(string);

		[Type(2, "map", typeof(MapSchema<Attribute>))]
        public MapSchema<Attribute> attributes = new MapSchema<Attribute>();
    }

	public partial class Entity
    {
        public SyncEntity OptimizeDataToSend()
        {
            SyncEntity entity = new SyncEntity(id, type);
            attributes.ForEach((key, value) =>
            {
                SyncAttribute att = new SyncAttribute();
                att.dataType = value.dataType;
                att.dataValue = value.dataValue;

                entity.attributes.Add(key, att);
            });

            return entity;
        }

        public void AddAttributeValue(string keyAtt, string type, object value)
        {
            Attribute att = new Attribute();
            att.dataType = type;
            att.dataValue = value.ToString();
            attributes.Add(keyAtt, att);
        }

        public void UpdateAttribute(string keyAtt, string value)
        {
            attributes[keyAtt].dataValue = value;
        }

        public T TryGetAndConvertAttribute<T>(string type, T defaultValue = default(T)) where T : System.IConvertible
        {
            if (attributes.ContainsKey(type))
            {
                string value = attributes[type].dataValue;
                return (T)System.Convert.ChangeType(value, typeof(T));
            }

            DebugExtension.LogError($"[Entity]: {type} attribute not found ");
            return defaultValue;
        }

        public bool TryGetAttribute(string type, out string value)
        {
            if (attributes.ContainsKey(type))
            {
                value = attributes[type].dataValue;
                return true;
            }

            DebugExtension.LogWarning($"[Entity]: {type} attribute not found ");
            value = default;
            return false;
        }
    }
}
