using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableMode : MonoBehaviour
{
    [SerializeField] private GameObject buttonSelectMode;
    [SerializeField] private Image borderDrawMode;
    [SerializeField] private Image bgDrawMode;
    [SerializeField] private Button btnDrawMode;
    [SerializeField] private Sprite sprBorderDrawMode;
    [SerializeField] private Sprite sprDrawMode;
    [SerializeField] private Sprite sprBtnDrawMode;

    // Start is called before the first frame update
    void Start()
    {
        if (GameContext.GameMode == GameMode.RealtimeMode)
        {
            DisableSelectMode();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisableSelectMode()
    {
        buttonSelectMode.SetActive(false);
        borderDrawMode.sprite = sprBorderDrawMode;
        bgDrawMode.sprite = sprDrawMode;
        var spriteState = btnDrawMode.spriteState;
        spriteState.pressedSprite = sprBtnDrawMode;
        spriteState.disabledSprite = sprBtnDrawMode;
        btnDrawMode.spriteState = spriteState;
    }
}
