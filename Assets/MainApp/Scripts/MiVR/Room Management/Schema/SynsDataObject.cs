// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas {
	public partial class SynsDataObject : Schema {
		[Type(0, "string")]
		public string content_name = default(string);

		[Type(1, "number")]
		public float currentDomeId = default(float);

		[Type(2, "array", typeof(ArraySchema<VRDome>))]
		public ArraySchema<VRDome> vr_dome_list = new ArraySchema<VRDome>();
	}
}
