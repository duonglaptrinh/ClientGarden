
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Setting : MonoBehaviour
{
    [SerializeField] Toggle toggleVoiceCall;
    [SerializeField] Toggle toggleShareBeam;

    //private DissonanceComms dissonanceComms;
    //private DissonanceComms DissonanceComms => dissonanceComms
    //    ? dissonanceComms
    //    : dissonanceComms = FindObjectOfType<DissonanceComms>();

    // Start is called before the first frame update
    private void Awake()
    {
        toggleVoiceCall.isOn = !SettingConfig.is_sharing_sound;
        toggleShareBeam.isOn = !SettingConfig.is_sharing_beam;
        OnOffVoice(toggleVoiceCall.isOn);
        OnOffBeam(toggleShareBeam.isOn);

        toggleVoiceCall.onValueChanged.AddListener(x => {
            OnOffVoice(x);
        });
        toggleShareBeam.onValueChanged.AddListener(x => {
            OnOffBeam(x);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnOffVoice(bool isOn)
    {
        //DissonanceComms.IsMuted = isOn;
        SettingConfig.is_sharing_sound = !isOn;
        toggleVoiceCall.GetComponentInChildren<Image>().enabled = !isOn;
    }

    void OnOffBeam(bool isOn)
    {
        SettingConfig.is_sharing_beam = !isOn;
        toggleShareBeam.GetComponentInChildren<Image>().enabled = !isOn;
    }
}
