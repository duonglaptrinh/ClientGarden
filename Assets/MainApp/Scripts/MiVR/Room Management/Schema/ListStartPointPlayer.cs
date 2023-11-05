// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.45
// 

using Colyseus.Schema;
using TWT.Model;

namespace SyncRoom.Schemas
{
    public partial class ListStartPointPlayer : Schema
    {
        [Type(0, "number")]
        public float indexStartPoint = default(float);

        [Type(1, "array", typeof(ArraySchema<StartPointPlayer>))]
        public ArraySchema<StartPointPlayer> listStartPoint = new ArraySchema<StartPointPlayer>();

    }
}
