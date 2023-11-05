// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;
using Newtonsoft.Json;

namespace SyncRoom.Schemas {
	public partial class Visitor : Schema {
		[Type(0, "string")]
		public string state = default(string);

		[Type(1, "string")]
		public string sessionId = default(string);

		[Type(2, "int32")]
		public int userId = default(int);

		[Type(3, "string")]
		public string name = default(string);

		[Type(4, "string")]
		public string userData = default(string);

	}
	
	public partial class Visitor
    {
		public Entity entity;
	}

}
