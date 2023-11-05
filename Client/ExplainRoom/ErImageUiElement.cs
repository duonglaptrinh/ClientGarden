using UnityEngine;
using UnityEngine.UI;

namespace Game.ExplainRoom
{
    public class ErImageUiElement : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        public void SetImage(Texture2D texture2D)
        {
            Rect rect = this.GetComponent<Image>().rectTransform.rect;
            Utility.SetImageWithRect(texture2D, image, rect);
        }

        public Texture GetTexture => image.texture;
    }
}