using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class SceneConfig
{
    public enum Scene
    {
        SplashScreen,
        ServerStartScene,
        ActiveUserScreen,
        TitleScreen,
        TitleScreenV2,
        ContentListScreen,
        BaseScreen,
        BaseScreenV2,
        AdminScreen,
        JoinRoom
    }
    
    public static void LoadScene(Scene scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single, Action onSceneLoaded = null)
    {
        SceneManager.LoadScene(scene.ToString(), loadSceneMode);
        
        if(onSceneLoaded == null) return;

        void CallBack(UnityEngine.SceneManagement.Scene s, LoadSceneMode mode)
        {
            if (s.name == scene.ToString())
            {
                onSceneLoaded.Invoke();
            }

            SceneManager.sceneLoaded -= CallBack;
        }

        SceneManager.sceneLoaded += CallBack;
    }

    public static void LoadScene(int scene_index)
    {
        SceneManager.LoadScene(scene_index);
    }

    public static bool IsCurrentScene(Scene scene)
    {
        return SceneManager.GetActiveScene().name.Equals(scene.ToString());
    }
}

