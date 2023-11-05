using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearSound : MonoBehaviour
{

    public AudioClip sound;

    private void OnEnable()
    {
        StartCoroutine(DoPlaySoundAfterFrame());
    }

    IEnumerator DoPlaySoundAfterFrame()
    {
        if (sound != null)
        {
            yield return null;
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
        }

    }
}
