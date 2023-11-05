using System;
using UnityEngine;

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        //DebugExtension.Log(DateTime.FromFileTimeUtc(jdt.value).ToString());
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        //DebugExtension.Log("Converted to JDT");
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}
