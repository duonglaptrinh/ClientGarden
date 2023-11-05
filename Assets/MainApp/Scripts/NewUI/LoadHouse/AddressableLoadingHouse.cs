using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using jp.co.mirabo.Application.RoomManagement;

public class AddressableLoadingHouse : SingletonMonoBehaviour<AddressableLoadingHouse>
{
    public static Action<GameObject[]> OnLoadSceneDone = null;
    //public AssetReference Scene;
    AsyncOperationHandle<SceneInstance> handle;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Awake()
    {
        if (Instance != null && Instance.GetInstanceID() != this.GetInstanceID())
        {
            //DebugExtension.Log("Another instance of " + GetType() + " is already exist! Destroying self...");
            DestroyImmediate(gameObject);
            return;
        }
    }
    //public void LoadSceneAsync(string scene)
    //{
    //    Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive).Completed += SceneLoadComplete;
    //}
    public async UniTask LoadSceneAsync(string scene)
    {
        var loadOperation = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
        var unityTask = loadOperation.ToUniTask();
        await unityTask;

        SceneLoadComplete(loadOperation);
    }
    void SceneLoadComplete(AsyncOperationHandle<SceneInstance> obj)
    {
        handle = obj;
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            DebugExtension.Log("Load Scene Done ...");
            OnLoadSceneDone?.Invoke(obj.Result.Scene.GetRootGameObjects());
        }
    }

    //public void UnloadScene(Action OnUnloadDone = null)
    //{
    //    Addressables.UnloadSceneAsync(handle, true).Completed += op =>
    //    {
    //        if (op.Status == AsyncOperationStatus.Succeeded)
    //        {
    //            Addressables.Release(handle);
    //            OnUnloadDone?.Invoke();
    //            DebugExtension.Log("Successfully unloaded Scene.");
    //        }
    //    };
    //}

    public async UniTask UnloadScene(Action OnUnloadDone = null)
    {
        if (handle.IsValid())
        {
            var unloadOp = Addressables.UnloadSceneAsync(handle, true);
            await unloadOp.Task;
            if (unloadOp.Status == AsyncOperationStatus.Succeeded)
            {
                OnUnloadDone?.Invoke();
                //Addressables.Release(handle);
                DebugExtension.Log("Successfully unloaded Scene.");
            }
        }
        else
        {
            OnUnloadDone?.Invoke();
            //Addressables.Release(handle);
            DebugExtension.Log("Handle is null.");
        }
    }
}
