// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas {
	public partial class TranformCamera : Schema {
		[Type(0, "ref", typeof(XYZ))]
		public XYZ position = new XYZ();

		[Type(1, "ref", typeof(XYZ))]
		public XYZ localScale = new XYZ();

		[Type(2, "ref", typeof(XYZ))]
		public XYZ eulerAngel = new XYZ();

		[Type(3, "ref", typeof(XYZW))]
		public XYZW rotation = new XYZW();
	}
}
