using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Extension;

public class Stage1ModelLoader : MonoBehaviour
{
    Dictionary<string, Transform> modelChilds = new Dictionary<string, Transform>();

    public Transform ModelCenter { get; protected set; }

	private string loadedModelName;
	private GameObject loadedModel;

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
                c.gameObject.GetOrAddComponent<Stage1AnswerHolder>();
                c.gameObject.GetOrAddComponent<SelectModelAsAnswer>();
                c.gameObject.GetOrAddComponent<MeshCollider>();

                modelChilds.Add(c.name, c);
            }
        }
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
                var v = modelChilds[name];

                var vholder = v.GetComponent<Stage1AnswerHolder>();
                if (vholder != null)
                    GameObject.Destroy(vholder);

                modelChilds.Remove(name);
            }

            var newcontainer = new GameObject(name).transform;

            newcontainer.SetParent(loadedModel.transform);
            newcontainer.position = center;

            foreach (var e in list)
            {
                var modelAsAnswer = e.GetComponent<SelectModelAsAnswer>();
                if (modelAsAnswer == null)
                    e.gameObject.AddComponent<SelectModelAsAnswer>();

                var vholder = e.GetComponent<Stage1AnswerHolder>();
                if (vholder != null)
                    GameObject.Destroy(vholder);

                e.transform.SetParent(newcontainer);
            }

            newcontainer.gameObject.AddComponent<Stage1AnswerHolder>();

            modelChilds.Add(name, newcontainer);
        }
        else
        {
            var newcontainer = new GameObject(name).transform;

            newcontainer.gameObject.AddComponent<Stage1AnswerHolder>();

            if (!modelChilds.ContainsKey(name))
            {
                modelChilds.Add(name, newcontainer);
            }
        }
    }

    public void BuildModelDependenciesTransparent(string model, params string[] dependencies)
    {
        if (!string.IsNullOrEmpty(model) && dependencies != null)
        {
            RemoveColliders(dependencies);

            Transform modelFound = null;

            if (modelChilds.TryGetValue(model, out modelFound))
            {
                var smaa = modelFound.GetComponent<Stage1AnswerHolder>();

                if (smaa != null)
                {
                    var list = new List<Stage1AnswerHolder>();

                    foreach (var e in dependencies)
                    {
                        if (string.IsNullOrEmpty(e))
                            continue;

                        Transform found = null;

                        if (modelChilds.TryGetValue(e, out found))
                        {
                            var esmma = found.GetComponent<Stage1AnswerHolder>();

                            if (esmma != null)
                            {
                                list.Add(esmma);
                            }
                        }
                    }

                    if (smaa.Dependencies == null)
                        smaa.Dependencies = list;
                    else
                        smaa.Dependencies.AddRange(list);
                }
            }
        }
    }

    public void RemoveColliders(params string[] names)
    {
        foreach (var e in modelChilds)
        {
            if (e.Value != null)
            {
                var colliders = e.Value.GetComponentsInChildren<Collider>();

                if (colliders != null)
                {
                    foreach (var collider in colliders)
                        collider.enabled = true;
                }

                // e.Value.gameObject.SetActive(true);
            }
        }

        if (modelChilds == null || modelChilds.Count == 0)
            return;

        if (names != null)
        {
            foreach (var e in names)
            {
                Transform found = null;

                if (modelChilds.TryGetValue(e, out found))
                {
                    var colliders = found.GetComponentsInChildren<Collider>();

                    if (colliders != null)
                    {
                        foreach (var collider in colliders)
                            collider.enabled = false;
                    }

                    // found.gameObject.SetActive(false);
                }
            }
        }
    }

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
            loadedModel.transform.position = position;
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        if (loadedModel != null)
        {
			loadedModel.transform.rotation = rotation;
		}
    }

    public void HideModelUp()
    {
        StopAllCoroutines();
        StartCoroutine(DoMoveUp(null));
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

            var moveDes = currentMoveUp.transform.position + Vector3.up * .69f;

            while (currentMoveUp.transform.position != moveDes)
            {
                currentMoveUp.transform.position =
                    Vector3.MoveTowards(currentMoveUp.transform.position, moveDes, Time.deltaTime * 2f);
                yield return null;
            }

            var top = moveDes + Vector3.up * .1f;
            var bottom = moveDes - Vector3.up * .1f;

            var direction = 1;

            while (true)
            {
                if (direction == 1 && currentMoveUp.transform.position.y >= top.y)
                {
                    direction = -1;
                }
                else if (direction == -1 && currentMoveUp.transform.position.y <= bottom.y)
                {
                    direction = 1;
                }

                currentMoveUp.transform.position = Vector3.MoveTowards(currentMoveUp.transform.position,
                    direction == 1 ? top : bottom, .5f * Time.deltaTime);

                yield return null;
            }
        }


        yield break;
    }

    public void StopMoveUpModel()
    {
        StopAllCoroutines();
        StartCoroutine(DoMoveUp(null));
    }

    private void OnDestroy()
    {
        ClearModel();
    }
}

public static class ModelLoaderHelper
{
    public static Transform DeepLoad(this Transform transform, List<string> names)
    {
        if (names != null && names.Count > 0)
        {
            var first = names[0];
            names.RemoveAt(0);

            if (!string.IsNullOrEmpty(first))
            {
                var foundChild = transform.Find(first);
                if (foundChild != null)
                {
                    if (names.Count == 0)
                        return foundChild;
                    else
                        return foundChild.DeepLoad(names);
                }
            }
        }

        return null;
    }
}

/*
public static class HighlightExtensions
{
    public static void AddOutLine(this Transform transform)
    {
        if(transform.GetComponent<MeshRenderer>()!=null)
        {
            // transform.gameObject.AddComponent<Outline>();
        }
    }
}
*/