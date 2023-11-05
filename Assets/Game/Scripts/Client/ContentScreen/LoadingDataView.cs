using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client.ContentScreen
{
    public class LoadingDataView : ViewComponentBase
    {
        [SerializeField] private Slider processBar;
        [SerializeField] private Text percentText;
        public float Value => processBar.value;
        public void UpdateProcess(float value)
        {
            percentText.text = $"{value:P}";
            processBar.value = value;
        }
    }
}