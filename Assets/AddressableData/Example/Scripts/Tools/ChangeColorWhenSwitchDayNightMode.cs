using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorWhenSwitchDayNightMode : MonoBehaviour
{
    [SerializeField] Color32 colorDay = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 colorNight = new Color32(157, 157, 157, 255);
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        ButtonDayNightMode.OnChangeDayNightMode += ChangeColor;
    }
    private void OnDisable()
    {
        ButtonDayNightMode.OnChangeDayNightMode -= ChangeColor;
    }
    public void ChangeColor()
    {
        Color32 color = GameContext.IsDayMode ? colorDay : colorNight;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var item in renderers)
        {
            foreach (var mat in item.materials)
            {
                mat.color = color;
            }
        }
    }
}
