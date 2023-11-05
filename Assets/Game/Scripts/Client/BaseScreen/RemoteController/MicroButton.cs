using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MicroButton : MonoBehaviour
{
    [SerializeField] GameObject btnOff;

    void Start()
    {
        //var dissonanceComms = FindObjectOfType<Dissonance.DissonanceComms>();
        //var muteMicBtn = GetComponent<Button>();
        //muteMicBtn.onClick.AddListener(delegate
        //{
        //    dissonanceComms.IsMuted = !dissonanceComms.IsMuted;
        //    btnOff.SetActive(dissonanceComms.IsMuted);
        //});  
    }

}
