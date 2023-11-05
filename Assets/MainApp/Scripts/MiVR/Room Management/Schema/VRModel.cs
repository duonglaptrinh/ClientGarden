// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas {
	public partial class VRModel : Schema {
		[Type(0, "number")]
		public float model_id = default(float);

		[Type(1, "string")]
		public string model_url = default(string);

		[Type(2, "string")]
		public string model_translate = default(string);

		[Type(3, "string")]
		public string model_rotation = default(string);

		[Type(4, "string")]
		public string model_scale = default(string);

		[Type(5, "string")]
		public string model_default_animation = default(string);

		[Type(6, "number")]
		public float vr_model_transparent_type = default(float);

		[Type(7, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> listMaterial = new ArraySchema<ObjectUnlocate>();

		[Type(8, "number")]
		public float nameTexture = default(float);

		[Type(9, "boolean")]
		public bool isOutline = default(bool);

		[Type(10, "string")]
		public string color = default(string);
		
		[Type(11, "boolean")]
		public bool isLock = default(bool);

		[Type(12, "string")]
		public string sessionId = default(string);
	}
}
