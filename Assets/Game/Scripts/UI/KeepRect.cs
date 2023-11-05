using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Client
{
    public class KeepRect : UIBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        private RectTransform rectTransformSelf;    

        private void Awake()
        {
            rectTransformSelf = GetComponent<RectTransform>();
        }

        private void Update()
        {
            rectTransformSelf.sizeDelta = rectTransform.sizeDelta;
        }
    }
}