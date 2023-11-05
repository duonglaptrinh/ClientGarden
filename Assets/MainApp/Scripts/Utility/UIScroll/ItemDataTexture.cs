using UnityEngine;

public class ItemDataTexture : ItemDataBase
{
    public string pathDataTexture;
    public int indexColor;

    /// <summary>
    /// use for new mode
    /// </summary>
    public ColorSet colorSet;
    public ItemDataTexture(string path, int indexColor)
    {
        this.pathDataTexture = path;
        this.indexColor = indexColor;
    }
    public ItemDataTexture(ColorSet colorSet, int indexColor)
    {
        this.colorSet = colorSet;
        this.indexColor = indexColor;
    }
}
