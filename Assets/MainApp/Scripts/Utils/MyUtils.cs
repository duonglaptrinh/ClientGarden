using System.Collections;
using System.Collections.Generic;
using ThirdOutline;
using UnityEngine;

public class MyUtils
{
    public static string FormatCurrency(int number)
    {
        return number.ToString("N0");
    }

    /// <summary>
    /// this function use GPU to scale and can not use on WebGL
    /// </summary>
    /// <param name="originalTexture"></param>
    /// <param name="targetSize"></param>
    /// <returns></returns>
    public static Texture2D ScaleTextureGPU(Texture2D originalTexture, int targetSize)
    {
        if (originalTexture != null)
        {
            originalTexture = Crop(originalTexture);

            Scale(originalTexture, targetSize, targetSize);

            return originalTexture;
        }
        else
        {
            Debug.LogError("Original texture is not assigned.");
            return null;
        }
    }

    public static Texture2D ScaleTextureV2(Texture2D originalTexture, int targetSize)
    {
        if (originalTexture != null)
        {
            //Get size with, height by ratio
            //float aspectRatio = (float)originalTexture.width / (float)originalTexture.height;
            //int targetHeight = targetSize;
            //int targetWidth = Mathf.RoundToInt(targetSize * aspectRatio);
            //originalTexture = ResizeTextureNormal(originalTexture, targetWidth, targetHeight);

            // ratio
            int width = originalTexture.width;
            int height = originalTexture.height;
            int square = 0;
            int sub = 0;

            if (width < height)
            {
                square = width;
                sub = height - width;
            }
            else
            {
                square = height;
                sub = width - height;
            }

            if (width < height)
            {
                originalTexture = CropTextureNormal(originalTexture, new Rect(0, sub / 2, square, square));
            }
            else
            {
                originalTexture = CropTextureNormal(originalTexture, new Rect(sub / 2, 0, square, square));
            }

            originalTexture = ResizeTextureNormal(originalTexture, targetSize, targetSize);

            return originalTexture;
        }
        else
        {
            Debug.LogError("Original texture is not assigned.");
            return null;
        }
    }
    public static Texture2D ScaleTextureFollowHeightRatio(Texture2D originalTexture, int targetSize)
    {
        if (originalTexture != null)
        {
            //Get size with, height by ratio
            float aspectRatio = (float)originalTexture.width / (float)originalTexture.height;
            int targetHeight = targetSize;
            int targetWidth = Mathf.RoundToInt(targetSize * aspectRatio);
            originalTexture = ResizeTextureNormal(originalTexture, targetWidth, targetHeight);

            return originalTexture;
        }
        else
        {
            Debug.LogError("Original texture is not assigned.");
            return null;
        }
    }
    public static Texture2D CropTextureNormal(Texture2D mTexture, Rect rect)
    {
        Color[] c = mTexture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        Texture2D m2Texture = new Texture2D((int)rect.width, (int)rect.height);

        m2Texture.SetPixels(c);
        m2Texture.Apply();

        return m2Texture;
    }

    static Texture2D ResizeTextureNormal(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, originalTexture.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = originalTexture.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    static Texture2D CropTexture2D(Texture2D originalTexture, Rect rect)
    {
        rect.x = 10;
        rect.y = 10;
        DebugExtension.Log(rect.x + " " + rect.y + " " + rect.width + " " + rect.height);
        int resizedWidth = (int)rect.width;
        int resizedHeight = (int)rect.height;
        RenderTexture renderTexture = new RenderTexture(originalTexture.width, originalTexture.height, 32);
        RenderTexture.active = renderTexture;
        Graphics.Blit(originalTexture, renderTexture);
        Texture2D resizedTexture = new Texture2D(resizedWidth, resizedHeight);
        resizedTexture.ReadPixels(rect, 0, 0);
        resizedTexture.Apply();
        return resizedTexture;
    }
    /// <summary>
    ///     Returns a scaled copy of given texture.
    /// </summary>
    /// <param name="tex">Source texure to scale</param>
    /// <param name="width">Destination texture width</param>
    /// <param name="height">Destination texture height</param>
    /// <param name="mode">Filtering mode</param>
    public static Texture2D Scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
    {
        Rect texR = new Rect(0, 0, width, height);
        _gpu_scale(src, width, height, mode);

        //Get rendered data back to a new texture
        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
        result.Resize(width, height);
        result.ReadPixels(texR, 0, 0, true);
        return result;
    }

    /// <summary>
    ///     Scales the texture data of the given texture.
    /// </summary>
    /// <param name="tex">Texure to scale</param>
    /// <param name="width">New width</param>
    /// <param name="height">New height</param>
    /// <param name="mode">Filtering mode</param>
    public static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
    {
        Rect texR = new Rect(0, 0, width, height);
        _gpu_scale(tex, width, height, mode);

        // Update new texture
        tex.Resize(width, height);
        tex.ReadPixels(texR, 0, 0, true);
        tex.Apply(true); //Remove this if you hate us applying textures for you :)
        DebugExtension.Log(tex.width + "  " + tex.height);
    }

    // Internal unility that renders the source texture into the RTT - the scaling method itself.
    private static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
    {
        DebugExtension.Log(src.width + "  " + src.height);
        //We need the source texture in VRAM because we render with it
        src.filterMode = fmode;
        src.Apply(true);

        //Using RTT for best quality and performance. Thanks, Unity 5
        RenderTexture rtt = new RenderTexture(width, height, 32);

        //Set the RTT in order to render to it
        Graphics.SetRenderTarget(rtt);

        //Setup 2D matrix in range 0..1, so nobody needs to care about sized
        GL.LoadPixelMatrix(0, 1, 1, 0);

        //Then clear & draw the texture to fill the entire RTT.
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
    }
    static Texture2D Crop(Texture2D texture2D)
    {
        RenderTexture image = new RenderTexture(texture2D.width, texture2D.height, 24, RenderTextureFormat.ARGB32);
        Graphics.Blit(texture2D, image);
        // Allocate a temporary RenderTexture with the original image dimensions
        RenderTexture rTex = RenderTexture.GetTemporary(image.width, image.height, 24, image.format);
        // Copy the original image
        Graphics.Blit(image, rTex);

        // Stores the size of the new square image
        int size;
        // Stores the coordinates in the original image to start copying from
        int[] coords;
        // Temporarily tores the new square image
        RenderTexture tempTex;

        if (image.width > image.height)
        {
            // Set the dimensions for the new square image
            size = image.height;
            // Set the coordinates in the original image to start copying from
            coords = new int[] { (int)((image.width - image.height) / 2f), 0 };
            // Allocate a temporary RenderTexture
            tempTex = RenderTexture.GetTemporary(size, size, 24, image.format);
        }
        else
        {
            // Set the dimensions for the new square image
            size = image.width;
            // Set the coordinates in the original image to start copying from
            coords = new int[] { 0, (int)((image.height - image.width) / 2f) };
            // Allocate a temporary RenderTexture
            tempTex = RenderTexture.GetTemporary(size, size, 24, image.format);
        }

        // Copy the pixel data from the original image to the new square image
        Graphics.CopyTexture(image, 0, 0, coords[0], coords[1], size, size, tempTex, 0, 0, 0, 0);

        // Free the resources allocated for the Temporary RenderTexture
        RenderTexture.ReleaseTemporary(rTex);
        // Allocate a temporary RenderTexture with the new dimensions
        rTex = RenderTexture.GetTemporary(size, size, 24, image.format);
        // Copy the square image
        Graphics.Blit(tempTex, rTex);

        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        // Free the resources allocated for the Temporary RenderTexture
        RenderTexture.ReleaseTemporary(tempTex);

        // Free the resources allocated for the Temporary RenderTexture
        RenderTexture.ReleaseTemporary(rTex);

        return tex;
    }
    private static string ConvertColor(Color color)
    {
        string colorString = ColorUtility.ToHtmlStringRGBA(color);
        return colorString;
    }
    public static string GetColorString(Renderer renderer)
    {
        Outline outline = renderer.GetComponent<Outline>();
        if (outline)
        {
            Color outlineColor = outline.OutlineColor;
            return ConvertColor(outlineColor);
        }
        return null;
    }
}
