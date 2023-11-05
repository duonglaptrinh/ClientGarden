using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.VGS;
using Game.Client;

public class BaseScreenTopMenuV2 : SingletonMonoBehavior<BaseScreenTopMenuV2>
{
    [SerializeField] private Button controlMenuButton = null;
    [SerializeField] private Button listItemButton = null;
    [SerializeField] private Button cameraButton = null;
    [SerializeField] private BaseScreenUiControllerV2 uiController = null;

    public bool isCameraRotating = true;
    private bool currentRotate = true;
    private void Start()
    {
        isCameraRotating = true;

        controlMenuButton.onClick.AddListener(OnClickControlMenuButton);
        listItemButton.onClick.AddListener(OnClickListItemButton);
        cameraButton.onClick.AddListener(OnClickCameraButton);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))//|| Input.GetKeyDown(KeyCode.Space))
        {
            //if (Cursor.lockState == CursorLockMode.Locked)
            // {
            // Cursor.lockState = CursorLockMode.None;
            PlayerManagerSwitch.IsAllowRotate = false;
            PlayerManagerSwitch.SetDefaultCursor();
            //  }
            //else
            //{
            //    playerController.IsAllowRotate = false;
            //    Cursor.lockState = CursorLockMode.Locked;

            //}
        }
        PlayerManagerSwitch.PopupShowing = VrDomeControllerV2.Instance.IsPopupShowing();
        if (!isCameraRotating)
            return;

        // TODO : Camera Rotate 

    }

    private void OnClickControlMenuButton()
    {
        uiController.HideShowUiController();
    }
    private void OnClickListItemButton()
    {
        uiController.HideShowListItemButton();
    }
    private void OnClickCameraButton()
    {
        uiController.HideShowCameraButton();
    }

    private void OnClickRotateCameraButton()
    {
        isCameraRotating = !isCameraRotating;
        currentRotate = isCameraRotating;
        //playerController.IsAllowRotate = isCameraRotating;
    }

    public void ResetCameraRotate()
    {
        isCameraRotating = currentRotate;
        //DebugExtension.Log(isCameraRotating);
        //playerController.IsAllowRotate = isCameraRotating;
    }

    public void SaveCurrentStatusCameraRotate()
    {
        currentRotate = isCameraRotating;
    }
    public void SetCameraRotate(bool value, bool interactable)
    {
        //DebugExtension.Log(isCameraRotating + " " + value);
        isCameraRotating = value;
        //playerController.IsAllowRotate = value;
    }

}
