using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRoomEditor : MonoBehaviour
{
    InputField roomField;
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        roomField = GetComponent<InputField>();
        roomField.text = RuntimeData.RoomID;
        roomField.onValueChanged.AddListener(text =>
        {
            RuntimeData.RoomID = text;
        });
    }

}
