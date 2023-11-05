using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffToggleButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image onBtn, offBtn;
    [SerializeField] bool init = false;
    void Start()
    {
        onBtn.enabled = init;
        offBtn.enabled = !init;
        var control = GetComponent<Toggle>();
        control.onValueChanged.AddListener(OnOffMode);
        control.isOn = init;
    }
    void OnOffMode(bool isOn)
    {
        onBtn.enabled = isOn;
        offBtn.enabled = !isOn;    
    }
}
