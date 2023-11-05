using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.Extension
{
    public static class GraphicExtension
    {
        public static Texture2D GetRTPixels(this RenderTexture rt)
        {
            // Remember currently active render texture
            RenderTexture currentActiveRT = RenderTexture.active;

            // Set the supplied RenderTexture as the active one
            RenderTexture.active = rt;

            // Create a new Texture2D and read the RenderTexture image into it
            Texture2D tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

            // Restorie previously active render texture
            RenderTexture.active = currentActiveRT;
            return tex;
        }

        public static Texture2D ScaleTexture(this Texture2D oldTexture, int width, int height)
        {
            Color[] oldTextureColors = oldTexture.GetPixels();
            Color[] newTextureColors = new Color[width * height];

            float ratioX = 1.0f / ((float)width / (oldTexture.width - 1));
            float ratioY = 1.0f / ((float)height / (oldTexture.height - 1));

            int oldWidth = oldTexture.width;
            int newWidth = width;

            for (int y = 0; y < height; y++)
            {
                int yFloor = Mathf.FloorToInt(y * ratioY);
                int y1 = yFloor * oldWidth;
                int y2 = (yFloor + 1) * oldWidth;
                int yWidth = y * newWidth;

                for (int x = 0; x < newWidth; x++)
                {
                    int xFloor = Mathf.FloorToInt(x * ratioX);
                    float xLerp = x * ratioX - xFloor;

                    newTextureColors[yWidth + x] = ColorLerpUnclamped(
                        ColorLerpUnclamped(oldTextureColors[y1 + xFloor], oldTextureColors[y1 + xFloor + 1], xLerp),
                        ColorLerpUnclamped(oldTextureColors[y2 + xFloor], oldTextureColors[y2 + xFloor + 1], xLerp),
                        y * ratioY - yFloor);
                }
            }

            Texture2D newTexture = new Texture2D(width, height);
            newTexture.SetPixels(newTextureColors);
            newTexture.Apply();
            return newTexture;
        }


        private static Color ColorLerpUnclamped(Color color1, Color color2, float v)
        {
            return new Color(color1.r + (color2.r - color1.r) * v,
                color1.g + (color2.g - color1.g) * v,
                color1.b + (color2.b - color1.b) * v,
                color1.a + (color2.a - color1.a) * v);
        }

        public static void SetRawImageWithRect(Texture texture, RawImage image, Rect rect)
        {
            if (image != null && texture != null && rect != null)
            {
                image.rectTransform.sizeDelta = GetNewSizeWithRect(new Vector2(texture.width, texture.height), rect);
                image.texture = texture;
            }
        }

        public static void SetImageWithRect(Texture2D texture, Image image, Rect rect)
        {
            if (image != null && texture != null && rect != null)
            {
                image.rectTransform.sizeDelta = GetNewSizeWithRect(new Vector2(texture.width, texture.height), rect);
                image.sprite = VRObjectV2.ConvertTexture2DToSprite(texture);
            }
        }

        public static Vector2 GetNewSizeWithRect(Vector2 origin, Rect rect)
        {
            float origin_rate = origin.x * 1f / origin.y;
            float rectRate = rect.width / rect.height;
            float newWidth = 1;
            float newHeight = 1;
            if (rectRate > origin_rate)
            {
                float rate = rect.height / origin.y;
                newWidth = (int)(origin.x * rate);
                newHeight = (int)rect.height;
            }
            else
            {
                float rate = rect.width / origin.x;
                newWidth = (int)rect.width;
                newHeight = (int)(origin.y * rate);
            }
            return new Vector2(newWidth, newHeight);
        }
    }
}