using UnityEngine;

public class ItemDataMaterial: ItemDataBase
{
    public Renderer renderer;
    public Material material;
    public ItemDataMaterial(Renderer rend, Material mat)
    {
        this.renderer = rend;
        this.material = mat;
    }
}
