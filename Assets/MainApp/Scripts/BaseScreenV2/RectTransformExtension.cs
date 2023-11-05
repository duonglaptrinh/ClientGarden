using UnityEngine;

namespace Shim.Utils
{
    // https://answers.unity.com/questions/888257/access-left-right-top-and-bottom-of-recttransform.html
    public static class RectTransformExtensions
    {
        public static void SetLeft(this Transform tr, float left)
        {
            RectTransform rt = (RectTransform)tr;
            SetLeft(rt, left);
        }
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this Transform tr, float right)
        {
            RectTransform rt = (RectTransform)tr;
            SetRight(rt, right);
        }
        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }
        public static void SetTop(this Transform tr, float top)
        {
            RectTransform rt = (RectTransform)tr;
            SetTop(rt, top);
        }
        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }
        public static void SetBottom(this Transform tr, float bottom)
        {
            RectTransform rt = (RectTransform)tr;
            SetBottom(rt, bottom);
        }
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }
}