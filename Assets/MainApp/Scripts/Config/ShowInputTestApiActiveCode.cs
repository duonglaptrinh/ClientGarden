using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowInputTestApiActiveCode : MonoBehaviour
{
    static bool isNew = true;
    // Start is called before the first frame update
    public void ShowInput(Transform activePanel)
    {
        if (isNew)
        {
            isNew = false;
        }
        GameObject prefab = LoadResourcesData.Instance.prefabApiInputTestTablet;
        GameObject obj = Instantiate(prefab, activePanel);
        obj.GetComponentInChildren<InputField>().text = UrlConfig.BASE_URL;
        obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            UrlConfig.BASE_URL = obj.GetComponentInChildren<InputField>().text;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

}
