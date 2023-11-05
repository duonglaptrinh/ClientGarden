using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VrgSyncApi;

public class TestVrgSyncApi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // var command = new AddCommand<Payload>("resource_path", new Payload());
        // var json = JsonConvert.SerializeObject(command);
        // DebugExtension.LogError(json);
        // DebugExtension.LogError(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json)));
    }

    public class Payload
    {
        public int x;
        public int y;
    }
}
