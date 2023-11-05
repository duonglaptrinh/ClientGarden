using Game.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDayNightMode : MonoBehaviour
{
    public static Action OnChangeDayNightMode = null;

    [SerializeField] Button btn;
    [SerializeField] Toggle toggleBtnNight;
    [SerializeField] Image icon;

    [SerializeField] Sprite iconDay;
    [SerializeField] Sprite iconNight;

    bool isShowPopup = false;

    private void OnEnable()
    {
        VRDomeLoadHouse.OnChangeHouseModel += OnChangeHouseModel;
    }
    private void OnDisable()
    {
        VRDomeLoadHouse.OnChangeHouseModel -= OnChangeHouseModel;
    }
    void OnChangeHouseModel(Transform transform)
    {
        Light[] m_Lights = FindObjectsOfType<Light>();
        //DebugExtension.LogError(m_Lights.Length);

        foreach (Light l in m_Lights)
        {
            if (l.type == LightType.Directional)
            {
                l.intensity = GameContext.IsDayMode ? 1 : 0.25f;
            }
            else if (l.type == LightType.Point)
            {
                if (!l.GetComponentInParent<VRModelV2>())
                    l.intensity += GameContext.IsDayMode ? 0.3f : -0.3f;
                else
                {
                    //DebugExtension.LogError(l.GetComponentInParent<ModelSupportCreatePrefab>().gameObject.name);
                }
            }
        }
        VRObjectManagerV2.Instance.ChangeColorModel();
    }

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() =>
        {
            ShowPopupAsk();
        });
        toggleBtnNight.isOn = GameContext.IsDayMode;
        //SettingDayNight();
        SettingDayNightUI();
    }

    void ShowPopupAsk()
    {
        if (isShowPopup) return; isShowPopup = true;
        string str = "夜に切り替えますか？";
        if (!GameContext.IsDayMode)
            str = "昼に切り替えますか？";

        PopupRuntimeManager.Instance.ShowPopup(str,
          onClickConfirm: () =>
          {
              isShowPopup = false;
              GameContext.IsDayMode = !GameContext.IsDayMode;
              SettingDayNight();
              SettingDayNightUI();
              toggleBtnNight.isOn = GameContext.IsDayMode;
          }, onClickCancel: () =>
          {
              isShowPopup = false;
              //DebugExtension.Log("Cancel");
          });
    }

    [ContextMenu("SettingDayNight")]
    public void Test()
    {
        GameContext.IsDayMode = !GameContext.IsDayMode;
        SettingDayNight();
    }
    public static void SettingDayNight()
    {
        VRDomeLoadHouse loadModel = VrDomeControllerV2.Instance.DomeLoadhouse;
        loadModel.ChangeDayOrNight(GameContext.IsDayMode);
        if (GameContext.IsDayMode)
        {
            //BaseScreenCtrl.LightmapChanger.Load("Day");
            RenderSettings.skybox = LoadResourcesData.Instance.skybox;
            RenderSettings.ambientSkyColor = new Color(0.212f, 0.227f, 0.259f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        }
        else
        {
            //BaseScreenCtrl.LightmapChanger.Load("Night");
            RenderSettings.ambientSkyColor = Color.black;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.skybox = LoadResourcesData.Instance.skyboxNight;
        }
        OnChangeDayNightMode?.Invoke();
    }

    void SettingDayNightUI()
    {
        icon.sprite = GameContext.IsDayMode ? iconDay : iconNight;
    }

    //void Update()
    //{
    //    if (GameContext.IsDayMode)
    //    {
    //        RenderSettings.skybox.Lerp(LoadResourcesData.Instance.skyboxNight, LoadResourcesData.Instance.skybox, .5f);
    //    }
    //    else
    //    {
    //        RenderSettings.skybox.Lerp(LoadResourcesData.Instance.skybox, LoadResourcesData.Instance.skyboxNight, .5f);
    //    }
    //    DynamicGI.UpdateEnvironment();
    //}
}
