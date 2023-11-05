using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.VGS;
using Takasho.GLD.VGS;

public class MenuSetting : MonoBehaviour
{
    public Slider viewSlider;
    public Slider speedSlider;
    public Slider hightSlider;

    GameObject editRemote;
    public Toggle toggleHigh;
    public Toggle toggleMiddle;
    public Toggle toggleLow;
    public int i ;

    public Button backBtn;

    public Text txtView ;
    public Text txtSpeed;
    public Text txtHight;

    // Start is called before the first frame update
    public void Awake()
    {
        SettingDefault();
        speedSlider.value = 4;
        Camera.main.fieldOfView = viewSlider.value;
        viewSlider.onValueChanged.AddListener(ChangeView);
        speedSlider.onValueChanged.AddListener(ChangeSpeed);
        hightSlider.onValueChanged.AddListener(ChangeHightd);
    }
    void Start()
    {
        i = QualitySettings.GetQualityLevel();
        SettingQualityStart();
        editRemote = BaseScreenUiControllerV2.Instance.EditRemote;
        backBtn.onClick.AddListener(BackToMainMenu);
        toggleHigh.onValueChanged.AddListener(value =>
        {
            if (value)
            {
                i = 5;
                QualitySettings.SetQualityLevel(i, true);
            }
        });
        toggleMiddle.onValueChanged.AddListener(value =>
        {
            if (value)
            {
                i = 3;
                QualitySettings.SetQualityLevel(i, true);
            }
        });
        toggleLow.onValueChanged.AddListener(value =>
        {
            if (value)
            {
                i = 1;
                QualitySettings.SetQualityLevel(i, true);
            }
        });
    }
  
    public void SettingDefault()
    {
        viewSlider.minValue = 60;
        viewSlider.maxValue = 90;
        viewSlider.value = 75;
        txtView.text = viewSlider.value.ToString("0");

        speedSlider.minValue = 2;
        speedSlider.maxValue = 6;
        speedSlider.value = 4;
        txtSpeed.text = speedSlider.value.ToString("0");

        hightSlider.minValue = 120;
        hightSlider.maxValue = 180;
        hightSlider.value = 160;
        txtHight.text = hightSlider.value.ToString("0") + "cm";
    }
    public void ChangeSpeed(float value)
    {
        PlayerManagerSwitch.speed = value;
        txtSpeed.text = value.ToString("0");
    }
    public void ChangeHightd(float value)
    {
        float hight = value / 100;
        var positionCam = Camera.main.transform.localPosition;
        positionCam.y = hight - 0.22f; //0.22 --> const - 1.6m ~~ 1.38m
        Camera.main.transform.localPosition = positionCam;// + new Vector3(0, hight - positionCam.y , 0);
        txtHight.text = value.ToString("0") + "cm";
    }

    public void ChangeView(float value)
    {
        Camera.main.fieldOfView = value;
        txtView.text = value.ToString("0");
    }
    public void SettingQualityStart()
    {
        if (i == 1)
            toggleLow.isOn = true;
        if (i == 5)
            toggleHigh.isOn = true;
        if (i == 3)
            toggleMiddle.isOn = true;
    }
    public void BackToMainMenu()
    {
        editRemote.SetActive(true);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
 
    private void OnEnable()
    {
        PlayerManagerSwitch.isShowMenu = true;
    }
    private void OnDisable()
    {
        PlayerManagerSwitch.isShowMenu = false;

    }
}
