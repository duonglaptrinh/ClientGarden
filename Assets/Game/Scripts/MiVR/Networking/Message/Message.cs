using System;
using UnityEngine;

namespace TWT.Networking
{
    [Serializable]
    public class MessagePayloadBase<T> where T : MessagePayloadBase<T>
    {
        public static string ToJson<T>(T mes) where T : MessagePayloadBase<T>
        {
            return JsonUtility.ToJson(mes);
        }

        public static T FromJson(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public string ToJson()
        {
            return ToJson((T)this);
        }
    }
    
}