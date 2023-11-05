using UnityEngine;

namespace Game.ExplainRoom
{
    public class ExplainRoomWavManager : ResourceLoadManagerBase<ErPlayWav>
    {
        public override ErPlayWav Load(string nameDefine, string path)
        {
            ErPlayWav erPlayWav;

            if (SourceLoadedMap.TryGetValue(nameDefine, out ErPlayWav erWav))
            {
                erPlayWav = erWav;
            }
            else
            {
                erPlayWav = new GameObject(nameDefine).AddComponent<ErPlayWav>();
                SourceLoadedMap.Add(nameDefine, erPlayWav);
            }

            erPlayWav.Initialize(path, true);
            erPlayWav.transform.parent = transform;
            
            return erPlayWav;
        }
    }
}