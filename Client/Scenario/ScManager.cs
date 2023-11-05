using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScManager : MonoBehaviour
{
    public void ClearAllScenarioObjects()
    {
        foreach(Transform child in transform)
            foreach (Transform c in child)
                Destroy(c.gameObject);
    }
}
