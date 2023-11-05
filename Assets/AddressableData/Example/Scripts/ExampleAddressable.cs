using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ExampleAddressable : MonoBehaviour
{
    [SerializeField] private GameObject loading;
    [SerializeField] private Text textTotal;
    [SerializeField] private Text textLoading;
    [SerializeField] private Image prefabThumb;
    [SerializeField] private ScrollRect scroll;
    private void OnEnable()
    {
        loading.SetActive(true);
        SetText(0);
        AddressableDownloadManager.ProgressEvent = OnProgress;
        AddressableDownloadManager.CompletionEvent = OnComplete;
    }

    void OnProgress(float progress)
    {
        SetText(progress);
    }
    void OnComplete(bool onComplete)
    {
        loading.SetActive(!onComplete);
        CreateScroll();
    }

    void SetText(float progress)
    {
        textLoading.text = "Loading " + progress + "%";
    }
    private void Update()
    {
        if (textTotal) textTotal.text = "Item Download: " + AddressableDownloadManager.Instance.CountItemDownload + "/" + AddressableDownloadManager.Instance.TotalItemDownload;
    }

    void CreateScroll()
    {
        List<Vector3> vecs = new List<Vector3>() { Vector3.right * 2, Vector3.right, Vector3.zero, -Vector3.right, -Vector3.right * 2 };
        int i = 0;
        //Assets/AddressableData/AllModel/5.ファニチャー/パルマアームチェアー/thumb.jpg
        foreach (var item in AddressableDownloadManager.ResourcesData.ListThumbPathAddressable)
        {
            int id = i;
            //string str = "Assets/AddressableData/AllModel/2.フェンス・スクリーン/トレメッシュフェンス/thumb1.jpg";
            Addressables.LoadAssetAsync<Sprite>(item).Completed += sprite =>
            {
                Image img = Instantiate(prefabThumb, scroll.content);
                img.sprite = sprite.Result;
            };
            string path = Path.GetDirectoryName(item);
            string pathAsset = Path.Combine(path, "data.asset");
            pathAsset = pathAsset.Replace("\\", "/");

            //DebugExtension.LogError(pathAsset);
            Addressables.LoadAssetAsync<OneModelAssetsData>(pathAsset).Completed += asset =>
            {
                OneModelAssetsData data = asset.Result;
                Addressables.InstantiateAsync(data.pathPrefab).Completed += model =>
                {
                    GameObject obj = model.Result;
                    obj.transform.position = vecs[id%vecs.Count];
                    DebugExtension.Log("InitializeAsync Model Done !!!");
                };
            };
            i++;
        }
    }
}
