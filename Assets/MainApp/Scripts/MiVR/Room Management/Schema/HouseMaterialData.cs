// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas {
	public partial class HouseMaterialData : Schema {
		[Type(0, "number")]
		public float indexHouse = default(float);

		[Type(1, "array", typeof(ArraySchema<MaterialSet>))]
		public ArraySchema<MaterialSet> ListMaterialSet = new ArraySchema<MaterialSet>();
	}
}
