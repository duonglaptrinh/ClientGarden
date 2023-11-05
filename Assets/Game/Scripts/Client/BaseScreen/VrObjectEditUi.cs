using System;
using UnityEngine;
using UnityEngine.UI;

public class VrObjectEditUi : MonoBehaviour
{
    [SerializeField] private Button viewBtn;
    [SerializeField] private Button editBtn;
    [SerializeField] private Button deleteBtn;
    [SerializeField] private GameObject transparentPanel;

    private void Start()
    {
//        throw new NotImplementedException();
    }

    public void ResetToDefault()
    {
        transparentPanel.SetActive(false);
    }
}