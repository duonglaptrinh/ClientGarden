// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas {
	public partial class VRObject : Schema {
		[Type(0, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> vr_mark_list = new ArraySchema<ObjectUnlocate>();

		[Type(1, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> vr_move_arrow_list = new ArraySchema<ObjectUnlocate>();

		[Type(2, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> vr_video_list = new ArraySchema<ObjectUnlocate>();

		[Type(3, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> vr_sound_list = new ArraySchema<ObjectUnlocate>();

		[Type(4, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> vr_image_list = new ArraySchema<ObjectUnlocate>();

		[Type(5, "array", typeof(ArraySchema<VRModel>))]
		public ArraySchema<VRModel> vr_model_list = new ArraySchema<VRModel>();

		[Type(6, "array", typeof(ArraySchema<ObjectUnlocate>))]
		public ArraySchema<ObjectUnlocate> vr_pdf_list = new ArraySchema<ObjectUnlocate>();
	}
}
