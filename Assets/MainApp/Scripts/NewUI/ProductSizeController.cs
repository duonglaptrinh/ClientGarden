using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProductSizeController : MonoBehaviour
{
    [SerializeField] private bool isFreeSize;
    public bool IsFreeSize => isFreeSize;
    [SerializeField] private int[] heightList;
    public int[] HeightList => heightList;
    [SerializeField] private int[] widthList;
    public int[] WidthList => widthList;
    [SerializeField] private int[] depthList;
    public int[] DepthList => depthList;
    [SerializeField] private List<SizeData> sizeList;
    public List<SizeData> SizeList => sizeList;

    public void ShowObjectSize(float height, float width, float depth)
    {
        // DebugExtension.LogError("ShowObjectSize = " + height + "  " + width + "  " + depth);
        //reset all
        sizeList.ForEach(x => x.Prefab.SetActive(false));

        SizeData size = sizeList.SingleOrDefault(x => x.Width == width && x.Height == height && x.Depth == depth);
        //foreach (SizeData sizeData in SizeList)
        //{
        //    if (sizeData.Width == width && sizeData.Height == height && sizeData.Depth == depth)
        //    {
        //        size = sizeData;
        //        break;
        //    }
        //}
        if (size != null)
        {
            //DebugExtension.LogError("Found ");
            size.Prefab.SetActive(true);
        }
        else
            sizeList[0].Prefab.SetActive(true);
    }
}

[Serializable]
public class SizeData
{
    [SerializeField] private int width;
    public int Width => width;
    [SerializeField] private int height;
    public int Height => height;
    [SerializeField] private int depth;
    public int Depth => depth;
    [SerializeField] private GameObject _prefab;
    public GameObject Prefab => _prefab;

}


