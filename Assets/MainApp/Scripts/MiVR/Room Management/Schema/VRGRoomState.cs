// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas {
	public partial class VRGRoomState : Schema {
		[Type(0, "number")]
		public float id = default(float);

		[Type(1, "string")]
		public string name = default(string);

		[Type(2, "string")]
		public string password = default(string);

		[Type(3, "map", typeof(MapSchema<Visitor>))]
		public MapSchema<Visitor> visitors = new MapSchema<Visitor>();

		[Type(4, "map", typeof(MapSchema<Entity>))]
		public MapSchema<Entity> entities = new MapSchema<Entity>();

		[Type(5, "string")]
		public string gameData = default(string);

		[Type(6, "number")]
		public float serverTime = default(float);

		[Type(7, "ref", typeof(SynsDataObject))]
		public SynsDataObject sync = new SynsDataObject();

	}
}
