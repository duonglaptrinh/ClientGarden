using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuUI : MonoBehaviour
{
    public Toggle toggleButtonEdit;
    // Start is called before the first frame update
    void Start()
    {
        toggleButtonEdit.onValueChanged.AddListener(OnOffEditMode);
    }
    private void OnEnable()
    {
        VRObjectManagerV2 manager = VRObjectManagerV2.Instance;
        toggleButtonEdit.isOn = manager.IsAllowShowUIEdit;
    }
    public void OnOffEditMode(bool isActive)
    {
        VRObjectManagerV2 manager = VRObjectManagerV2.Instance;
        if (isActive == manager.IsAllowShowUIEdit) return;
        //manager.IsAllowShowUIEdit = isActive;
        //DebugExtension.LogError("OnOffEditMode = " + manager.IsAllowShowUIEdit);
        foreach (var item in manager.GetComponentsInChildren<VRObjectV2>())
        {
            //item.UiButtons.gameObject.SetActive(isActive);
            //item.OnOffDrag(manager.IsAllowShowUIEdit);
        }
    }
}
