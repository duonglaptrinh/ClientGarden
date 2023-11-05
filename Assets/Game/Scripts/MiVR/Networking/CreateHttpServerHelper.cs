using System;
using System.IO;
using Game.Client;
using TWT.Networking.Server;
using TWT.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateHttpServerHelper : SingletonMonoBehavior<CreateHttpServerHelper>
{
    [SerializeField] private string myFolder = "/Users/apple/Documents/BuildUnity/TWT/Dummy/Server/VrHostContent/";

    private async void Start()
    {
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        myFolder = $"{Application.dataPath}/../";
#elif UNITY_STANDALONE_WIN
        myFolder = $"{Application.dataPath}/../../";
#elif UNITY_STANDALONE_OSX
        myFolder = $"{Application.dataPath}/../../";
#endif
        var savedFolder = PlayerPrefs.GetString(PlayerPrefsConstant.DATA_FOLDER, "");
        if (!string.IsNullOrEmpty(savedFolder))
            myFolder = savedFolder;
        else
        {
            myFolder = Path.GetFullPath(myFolder);
            myFolder = myFolder.Substring(0, myFolder.Length - 1);
        }
        GameContext.VrResourceRootPathOnServer = myFolder;

#if UNITY_STANDALONE

        var path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            // try
            // {
            //     await VrResourceStruct.ValidateAsync(path);
            // }
            // catch
            // {
            //     DebugExtension.LogError("Validated data fail!");
            //     Application.Quit(3);
            //     return;
            // }

            // var resourceLocalPath =
            //     Path.Combine(Application.persistentDataPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT);
            // var resourceServerPath = Path.Combine(path, VrDomeAssetResourceNameDefine.VR_DATA_ROOT);
            
            // FileUtility.DeleteAllFile(resourceLocalPath);

            // if (Directory.Exists(resourceLocalPath))
            // {
            //     Directory.Delete(resourceLocalPath);
            // }
            
            // FileUtility.DirectoryCopy(resourceServerPath, resourceLocalPath, true);

            HttpServerComponent.Instance.StartHttpServer(path, GameContext.API_PORT);
        }
        else
        {
            throw new FileNotFoundException(path);
        }
#endif

#if UNITY_STANDALONE
        SceneConfig.LoadScene(SceneConfig.Scene.BaseScreen);
#endif
    }
}