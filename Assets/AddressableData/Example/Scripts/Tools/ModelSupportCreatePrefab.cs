using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSupportCreatePrefab : MonoBehaviour
{
    [SerializeField] bool isNeedAutoCreateMeshCollider = true;
    public bool IsNeedAutoCreateMeshCollider => isNeedAutoCreateMeshCollider;
    /// <summary>
    /// Index same listPathColorTexture 
    /// </summary>
    public List<GameObject> ListGameObjectbByColor = new List<GameObject>();
    public void ShowModel(int indexColor)
    {
        for (int i = 0; i < ListGameObjectbByColor.Count; i++)
        {
            ListGameObjectbByColor[i].gameObject.SetActive(i == indexColor);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    [ContextMenu("Set Position Model")]
    public void SetPositionModel()
    {
        int count = transform.childCount;
        if (count <= 0)
        {
            DebugExtension.LogError("Model is null");
            return;
        }
        Transform model = transform.GetChild(0);
        Bounds modelBounds = CalculateLocalBounds(model.gameObject);
        Vector3 pos = new Vector3(modelBounds.size.x / 2, 0, modelBounds.size.y / 2);
        for (int i = 0; i < count; i++)
        {
            Transform m = transform.GetChild(i);
            m.transform.localPosition = pos;
            m.transform.localScale = Vector3.one * 100;
        }
        SaveEditor(this);

    }
    [ContextMenu("Save Data")]
    public static void SaveEditor(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
    public static Bounds CalculateLocalBounds(GameObject gameObj)
    {
        Quaternion currentRotation = gameObj.transform.rotation;
        gameObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(gameObj.transform.position, Vector3.zero);

        foreach (Renderer renderer in gameObj.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - gameObj.transform.position;
        bounds.center = localCenter;
        //DebugExtension.Log("The local bounds of this model is " + bounds);

        gameObj.transform.rotation = currentRotation;

        return bounds;
    }
}
