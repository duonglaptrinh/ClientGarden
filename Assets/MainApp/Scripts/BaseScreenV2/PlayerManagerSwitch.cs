using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.VGS;
using SyncRoom.Schemas;
using System;

public class PlayerManagerSwitch : MonoBehaviour
{
#if UNITY_WEBGL
    public static PlayerController playerController;
    public static Vector3 position => playerController._controller.transform.position;
    public static Vector3 localScale => playerController._controller.transform.localScale;
    public static Vector3 eulerAngles => playerController._controller.transform.eulerAngles;
    public static Quaternion rotation => playerController._controller.transform.rotation;
    public static bool IsAllowMove
    {
        get
        {
            return playerController.IsAllowMove;
        }
        set
        {
            //Task 740 - dont need lock move
            //playerController.IsAllowMove = value;
            playerController.IsAllowMove = true;
        }
    }

    public static bool IsAllowRotate
    {
        get => playerController.IsAllowRotate;
        set
        {
            //Task 740 - dont need lock Rotate
            //playerController.IsAllowRotate = value;
            playerController.IsAllowRotate = true;
        }
    }
    public static float speed
    {
        get => PlayerController.speed;
        set => PlayerController.speed = value;
    }
    public static bool isDrag
    {
        get => PlayerController.isDrag;
        set => PlayerController.isDrag = value;
    }
    public static bool isEdit
    {
        get => PlayerController.isEdit;
        set => PlayerController.isEdit = value;
    }
    public static bool isShowMenu
    {
        get => PlayerController.isShowMenu;
        set => PlayerController.isShowMenu = value;
    }
    public static bool PopupShowing
    {
        get => playerController.PopupShowing;
        set => playerController.PopupShowing = value;
    }
    public static bool canFly
    {
        get => playerController.canFly;
        set => playerController.canFly = value;
    }
    public static Transform myTransform
    {
        get => playerController.transform;
    }
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public static void SetPosition(string model_translate)
    {
        SetPosition(VRObjectManagerV2.ConverStringToVector3(model_translate));
    }
    public static void SetPosition(Vector3 model_translate)
    {
        playerController.transform.position = model_translate;
    }

    public static void EnableCharacter()
    {
        playerController.ActiveCharacter(true);
    }
    public static void DisableCharacter()
    {
        playerController.ActiveCharacter(false);
    }
   
    public static void SetRotation(string model_rotation)
    {
        SetRotation(VRObjectManagerV2.ConverStringToVector3(model_rotation));
    }
    public static void SetRotation(Vector3 model_rotation)
    {
        playerController._controller.transform.eulerAngles = model_rotation;
    }
    public static void SetRotation(Quaternion model_rotation)
    {
        playerController._controller.transform.rotation = model_rotation;
    }
    public static void SetDefaultCursor()
    {
        playerController.SetDefaultCursor();
    }
#endif

#if UNITY_ANDROID 
    public static CharacterController playerController;
    public static OVRPlayerController ovrPlayerController;
    public static bool IsAllowMove
    {
        get
        {
            return playerController.enabled;
        }
        set
        {
            playerController.enabled = value;
        }
    }

    static bool isAllowRotate = true;
    public static bool IsAllowRotate
    {
        get => isAllowRotate;
        set => isAllowRotate = value;
    }
    public static float speed
    {
        get => ovrPlayerController.Damping;
        set => ovrPlayerController.Damping = value;
    }
    public static bool isDrag { get; set; }

    public static bool isEdit { get; set; }

    public static bool isShowMenu { get; set; }

    public static bool PopupShowing { get; set; }

    public static bool canFly { get; set; }

    public static Transform myTransform
    {
        get => playerController.transform;
    }
    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
        ovrPlayerController = GetComponent<OVRPlayerController>();
    }

    public static void SetPosition(string model_translate)
    {
        playerController.transform.position = VRObjectManagerV2.ConverStringToVector3(model_translate);
    }
    public static void SetRotation(string model_rotation)
    {
        playerController.transform.eulerAngles = VRObjectManagerV2.ConverStringToVector3(model_rotation);
    }
    public static void SetDefaultCursor()
    {
        //playerController.SetDefaultCursor();
    }
#endif
}
