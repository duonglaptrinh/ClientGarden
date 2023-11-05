using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductColorController : MonoBehaviour
{
    [SerializeField] private List<ColorSet> listColorSets;
    public List<ColorSet> ListColorSets => listColorSets;

    Renderer[] listRenderers;
    public void Setup(Renderer[] rens)
    {
        listRenderers = rens;
    }
    public void UpdateMaterial(int indexColor)
    {
        ColorSet colorSet = listColorSets[indexColor];
        foreach (Renderer renderer in listRenderers)
        {
            var list = colorSet.ListMaterials;
            var materials = renderer.materials;
            foreach (var mat in list)
            {
                materials[mat.MaterialIndex] = mat.ObjectMaterial;
               // DebugExtension.LogError(materials[mat.MaterialIndex].name);
            }
            renderer.materials = materials;
        }
    }
}

[Serializable]
public class ColorSet
{
    [SerializeField] private string colorName;
    public string ColorName => colorName;

    [SerializeField] private Texture colorThumbnail;
    public Texture ColorThumbnail => colorThumbnail;

    [SerializeField] private List<ObjectMaterialSet> listMaterials;
    public List<ObjectMaterialSet> ListMaterials => listMaterials;
}

[Serializable]
public class ObjectMaterialSet
{
    [SerializeField] private int materialIndex;
    public int MaterialIndex => materialIndex;
    [SerializeField] private Material objectMaterial;
    public Material ObjectMaterial => objectMaterial;
}