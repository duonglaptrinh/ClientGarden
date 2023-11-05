using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickSound : MonoBehaviour, IPointerClickHandler
{
    public AudioClip sound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
    }
}
