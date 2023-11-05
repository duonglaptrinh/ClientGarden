using Game.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPositionCustom : MonoBehaviour
{
    [SerializeField] private float targetXLocalPosition = 0;
    [SerializeField] private float targetYLocalPosition = 0;
    public void SetPosition(VrInputKeyBoard keyboard)
    {
        keyboard.transform.localPosition = new Vector3(targetXLocalPosition, targetYLocalPosition, keyboard.transform.localPosition.z);
    }
}
