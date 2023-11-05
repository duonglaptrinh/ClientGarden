// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;

namespace SyncRoom.Schemas
{
    public partial class ModelData : Schema
    {
        [Type(0, "number")]
        public float indexHouse = default(float);

        [Type(1, "string")]
        public string model_translate = default(string);

        [Type(2, "string")]
        public string model_rotation = default(string);

        [Type(3, "string")]
        public string model_scale = default(string);

        [Type(4, "array", typeof(ArraySchema<HouseMaterialData>))]
        public ArraySchema<HouseMaterialData> ListHouseMaterialData = new ArraySchema<HouseMaterialData>();

        [Type(5, "number")]
        public float Land_Setting_FrontOf = default(float);

        [Type(6, "number")]
        public float Land_Setting_Behide = default(float);

        [Type(7, "number")]
        public float Land_Setting_Left = default(float);

        [Type(8, "number")]
        public float Land_Setting_Right = default(float);
    }
}
