using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestCamera360Capture : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            byte[] bytes = I360Render.Capture(4096, true);
            File.WriteAllBytes(Application.dataPath + "/SavedScreen.jpg", bytes);
        }
    }
}
