using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelLoader : MonoBehaviour
{
    Dictionary<string, Transform> modelChilds = new Dictionary<string, Transform>();

    public void LoadModel(string name, Vector3 position)
    {
        if (loadedModelName != name)
            ClearModel();

        var prefab = Resources.Load<GameObject>(name);

        if (prefab == null)
            return;

        var obj = GameObject.Instantiate(prefab) as GameObject;
        loadedModel = obj;

        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = position;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            var c = obj.transform.GetChild(i);
            if (!modelChilds.ContainsKey(c.name))
            {
                modelChilds.Add(c.name, c);
            }
        }

        loadedModel.SetActive(false);
    }

    public void ModelDefine(string name, string[] childs)
    {
        if (loadedModel == null)
            return;

        var list = new List<Transform>();

        if (childs != null && childs.Length > 0)
        {
            foreach (var e in childs)
            {
                var splits = e.Split('-');
                if (splits == null || splits.Length == 0)
                {
                    continue;
                }

                var t = loadedModel.transform.DeepLoad(splits.ToList());

                if (t != null)
                {
                    list.Add(t);
                }
            }
        }

        if (list.Count > 0)
        {
            var totalV3 = Vector3.zero;

            foreach (var e in list)
            {
                if (modelChilds.ContainsKey(e.gameObject.name))
                    modelChilds.Remove(e.gameObject.name);

                totalV3 += e.position;
            }

            var center = totalV3 / list.Count;


            if (modelChilds.ContainsKey(name))
            {
                modelChilds.Remove(name);
            }

            var newcontainer = new GameObject(name).transform;

            newcontainer.SetParent(loadedModel.transform);
            newcontainer.position = center;

            foreach (var e in list)
            {
                e.transform.SetParent(newcontainer);
            }

            modelChilds.Add(name, newcontainer);
        }
        else
        {
            var newcontainer = new GameObject(name).transform;

            if (!modelChilds.ContainsKey(name))
            {
                modelChilds.Add(name, newcontainer);
            }
        }
    }

    private string loadedModelName;

    private GameObject loadedModel;

    [SerializeField] Transform targetModelHud;

    public void ClearModel()
    {
        if (loadedModel != null)
        {
            GameObject.Destroy(loadedModel);
        }

        foreach (var e in modelChilds.Values)
        {
            if (e != null)
            {
                GameObject.Destroy(e.gameObject);
            }
        }

        modelChilds.Clear();
    }

    public void SetPosition(Vector3 position)
    {
        if (loadedModel != null)
        {
            loadedModel.transform.localPosition = position;
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        if (loadedModel != null)
        {
            loadedModel.transform.localRotation = rotation;
        }
    }

    private void OnDestroy()
    {
        ClearModel();
    }

    Transform currentModelShowOnHud;

    public void ShowModelOnHUd(string name)
    {
        Debug.LogError("ShowModelOnHUd " + name);

        if (currentModelShowOnHud != null && currentModelShowOnHud.name == name)
            return;

        if (string.IsNullOrEmpty(name))
            return;

        Transform found = null;

        modelChilds.TryGetValue(name, out found);

        if (currentModelShowOnHud != null)
        {
            currentModelShowOnHud.SetParent(loadedModel.transform);
        }

        if (found != null)
        {
            Debug.LogError("Found object need show on hud");

            currentModelShowOnHud = found;

            // StartCoroutine(DoMoveUp(found.gameObject));
            currentModelShowOnHud.transform.SetParent(targetModelHud.transform);

            currentModelShowOnHud.transform.localPosition = Vector3.zero;

        }
    }

    public void ModelUp(string name)
    {
        Debug.LogError("Model up " + name);

        if (currentMoveUp != null && currentMoveUp.name == name)
            return;

        StopAllCoroutines();

        if (string.IsNullOrEmpty(name))
            return;

        var found = loadedModel.transform.Find(name);

        if (found != null)
        {
            Debug.LogError("Found object need up");

            StartCoroutine(DoMoveUp(found.gameObject));
        }

    }

    Vector3 cachedCurrentMoveUpPosition;
    Quaternion cachedCurrentMoveUpRotation;

    GameObject currentMoveUp;
    IEnumerator DoMoveUp(GameObject target)
    {
        if (currentMoveUp != null)
        {
            currentMoveUp.transform.localPosition = cachedCurrentMoveUpPosition;
            currentMoveUp.transform.localRotation = cachedCurrentMoveUpRotation;
        }

        currentMoveUp = target;

        if (currentMoveUp != null)
        {
            cachedCurrentMoveUpPosition = currentMoveUp.transform.localPosition;
            cachedCurrentMoveUpRotation = currentMoveUp.transform.localRotation;
        }

        var moveDes = currentMoveUp.transform.position + Vector3.up * .69f;

        while (currentMoveUp.transform.position != moveDes)
        {
            currentMoveUp.transform.position = Vector3.MoveTowards(currentMoveUp.transform.position, moveDes, Time.deltaTime * 2f);
            yield return null;
        }

        while (true)
        {
            currentMoveUp.transform.RotateAround(currentMoveUp.transform.position, Vector3.down, Time.deltaTime * 360f);
            yield return null;
        }

        yield break;
    }
}
