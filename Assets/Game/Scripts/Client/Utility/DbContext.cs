using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DbContext
{
    private static DbContext instance { get; set; }
    public static DbContext Instance
    {
        get
        {
            if (instance == null) instance = new DbContext();
            return instance;
        }
    }

    private Dictionary<string, object> ModelData;

    public DbContext()
    {
        ModelData = new Dictionary<string, object>();
    }

    public void Remove<T>()
    {
        if (!ModelData.ContainsKey(typeof(T).Name)) return;
        ModelData.Remove(typeof(T).Name);
    }

    public void Set<T>(T data, string key = "")
    {
        if (string.IsNullOrEmpty(key))
            ModelData[typeof(T).Name] = data;
        else
            ModelData[key] = data;
    }

    public T Get<T>(string key = "")
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;
        if (!ModelData.ContainsKey(key)) return default(T);
        return (T)ModelData[key];
    }

    public bool Exists<T>(string key = "")
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;
        return ModelData.ContainsKey(key) && ModelData[key] != null;
    }
}