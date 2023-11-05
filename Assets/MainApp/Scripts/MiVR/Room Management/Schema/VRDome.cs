// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas
{
    public partial class VRDome : Schema
    {
        [Type(0, "number")]
        public float dome_id = default(float);

        [Type(1, "string")]
        public string d360_file_name = default(string);

        [Type(2, "ref", typeof(ModelData))]
        public ModelData modelData = new ModelData();

        [Type(3, "string")]
        public string d360_file_type = default(string);

        [Type(4, "boolean")]
        public bool d360_video_loop = default(bool);

        [Type(5, "number")]
        public float dome_size = default(float);

        [Type(6, "number")]
        public float dome_rotate = default(float);

        [Type(7, "ref", typeof(VRObject))]
        public VRObject vr_object_list = new VRObject();

        [Type(8, "ref", typeof(TranformCamera))]
        public TranformCamera tranformCamera = new TranformCamera();

        [Type(9, "ref", typeof(ListStartPointPlayer))]
        public ListStartPointPlayer listStartPointData = new ListStartPointPlayer();

        [Type(10, "number")]
        public float id_url = default(float);
    }
}
