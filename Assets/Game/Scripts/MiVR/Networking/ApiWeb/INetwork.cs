using Game.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using UnityEngine.Networking;

public class INetwork : MonoBehaviour
{
    private static INetwork m_Instance;

    public static INetwork instance
    {
        get
        {
            if (m_Instance == null)
            {
                // Search for existing instance.
                m_Instance = (INetwork)FindObjectOfType(typeof(INetwork));

                // Create new instance if one doesn't already exist.
                if (m_Instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    m_Instance = singletonObject.AddComponent<INetwork>();
                    singletonObject.name = typeof(INetwork).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return m_Instance;
        }
    }

    [HideInInspector]
    public string firebaseToken = string.Empty;
    [HideInInspector]
    public string refreshToken = "";

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void OnRequest(RequestType type, string method, Dictionary<string, object> form = null, bool isAutherntication = false, ICallback callback = null, bool isParams = false, ICallback error = null, bool isLoading = true, bool isCustomURL = false, bool isHideLoading = true)
    {
        StartCoroutine(Request(type, method, form, isAutherntication, callback, isParams, error, isLoading, isCustomURL, isHideLoading));
    }

    IEnumerator Request(RequestType type, string method, Dictionary<string, object> form = null, bool isAutherntication = false, ICallback callback = null, bool isParams = false, ICallback error = null, bool isLoading = true, bool isCustomURL = false, bool isHideLoading = true)
    {
        UnityWebRequest www = null;
        switch (type)
        {
            case RequestType.GET:
            case RequestType.DELETE:
                string dataRequest = string.Empty;
                if (form != null)
                {
                    if (form.Count == 1)
                    {
                        if (!isParams)
                        {
                            var first = form.FirstOrDefault();
                            dataRequest += "/" + first.Value;
                        }
                        else
                        {
                            for (int i = 0; i < form.Count; i++)
                            {
                                string sign = i == 0 ? "?" : "&";
                                string key = form.Keys.ElementAt(i);
                                dataRequest += sign + key + "=" + form[key];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < form.Count; i++)
                        {
                            string sign = i == 0 ? "?" : "&";
                            string key = form.Keys.ElementAt(i);
                            dataRequest += sign + key + "=" + form[key];
                        }
                    }
                }
                if (type == RequestType.GET)
                {
                    if (isCustomURL)
                        www = UnityWebRequest.Get(string.Format("{0}{1}", method, dataRequest));
                    else
                        www = UnityWebRequest.Get(string.Format("{0}{1}{2}", UrlConfig.BASE_URL, method, dataRequest));
                }
                else if (type == RequestType.DELETE)
                {
                    if (isCustomURL)
                        www = UnityWebRequest.Delete(string.Format("{0}{1}", method, dataRequest));
                    else
                        www = UnityWebRequest.Delete(string.Format("{0}{1}{2}", UrlConfig.BASE_URL, method, dataRequest));
                }
                break;
            case RequestType.POST:
            case RequestType.PUT:
                WWWForm dataForm = null;
                if (form != null)
                {
                    dataForm = new WWWForm();
                    dataForm.headers["Content-Type"] = "application/json";
                    foreach (var item in form)
                    {
                        string value = item.Value.ToString();
                        dataForm.AddField(item.Key, value);
                    }
                }
                if (type == RequestType.POST)
                {
                    if (isCustomURL)
                        www = UnityWebRequest.Post(method, dataForm);
                    else
                        www = UnityWebRequest.Post(string.Format("{0}{1}", UrlConfig.BASE_URL, method), dataForm);
                }
                else if (type == RequestType.PUT)
                {
                    if (dataForm != null)
                    {
                        if (isCustomURL)
                            www = UnityWebRequest.Put(method, dataForm.data);
                        else
                            www = UnityWebRequest.Put(string.Format("{0}{1}", UrlConfig.BASE_URL, method), dataForm.data);
                    }
                    else
                    {
                        byte[] byteArray = null;
                        if (isCustomURL)
                            www = UnityWebRequest.Put(method, byteArray);
                        else
                            www = UnityWebRequest.Put(string.Format("{0}{1}", UrlConfig.BASE_URL, method), byteArray);
                    }
                }

                www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                break;
            default:
                yield return null;
                break;
        }

        DebugExtension.Log(www.url.ToString());

        //Add headers
        if (isAutherntication)
        {
            Token token = DbContext.Instance.Get<Token>();
            if (token == null)
            {
                DebugExtension.Log("AccessToken missing! Can't request!!!!!!");
                yield return null;
            }
            www.SetRequestHeader("authorization", string.Format("{0} {1}", token.data.token_type, token.data.access_token));
        }
        www.downloadHandler = new DownloadHandlerBuffer();
        www.chunkedTransfer = false;
        www.useHttpContinue = false;
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            long statusCode = www.responseCode;
            string response = www.downloadHandler.text;
            //if (form != null)
            //    foreach (KeyValuePair<string, object> kvp in form)
            //        DebugExtension.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            //DebugExtension.Log(string.Format("RESPONSE {0} {1} {2} {3},\n{4}", type, method, statusCode, www.error, response));

            if (statusCode == 422 || statusCode == 404 || statusCode == 400 || statusCode == 500 || statusCode == 401 || statusCode == 901 || statusCode == 902 || statusCode == 903 || statusCode == 904)
            {
                error?.Invoke(statusCode, response);
            }
            else
            {
                error?.Invoke(0, response);
            }

        }
        else
        {
            long statusCode = www.responseCode;
            string response = www.downloadHandler.text;
            //if (form != null)
            //    foreach (KeyValuePair<string, object> kvp in form)
            //        DebugExtension.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            //DebugExtension.Log(string.Format("RESPONSE {0} {1} {2} {3}", type, method, www.responseCode, response));

            if (statusCode == 200 || statusCode == 201)
            {
                callback?.Invoke(statusCode, response);
            }
            else
            {
                error?.Invoke(statusCode, response);
            }
        }
    }

    public void ShowNoInternetPopup(Action confirm)
    {
        PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("インターネット接続がありません", confirm);
    }
}

public enum RequestType
{
    NONE,
    GET,
    POST,
    PUT,
    DELETE
}

#region Token
public class Token
{
    public int code;
    public TokenData data;
    public string message;
}

[System.Serializable]
public class TokenData
{
    public string token_type;
    public int expires_in;
    public string access_token;
    public string refresh_token;
}
#endregion

#region Error
public class ErrorData
{

    public int code;
    public ErrorFullData data;
    public string[] message;
}

public class Error
{
    public int code;
    public ErrorFullData data;
    public string message;
}

[System.Serializable]
public class ErrorFullData
{
    public int check;
}
#endregion