using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOfflineModeTitle : MonoBehaviour
{
    [SerializeField] GameObject[] offlineTitles;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var title in offlineTitles)
        {
            title.SetActive(GameContext.IsOffline && !GameContext.IsDemo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
