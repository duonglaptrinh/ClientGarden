using System;
using System.Collections;
using System.Collections.Generic;
using TWT.Utility;
using UnityEngine;

using UnityEngine.Networking;

public class ApiController
{
    public static void SendCodeRequest(string api, string code, string device_id, int gameMode, Action<CodeData> callback, Action errorCallback, Action onTimeOutOffline = null, Action onCheckOnlineFail = null)
    {
        var type = (int)UserType.Other;
#if UNITY_STANDALONE
        type = (int)UserType.PC;
#endif

        var form = new Dictionary<string, object>();
        form.Add("code", code);
        form.Add("mac_address", device_id);
        form.Add("type", type);
        form.Add("mode", gameMode);
        form.Add("product", ProductType.XR360);
        INetwork.instance.OnRequest(RequestType.POST, api, form, false,
        (status, data) =>
        {
            CodeRespone dataRespone = JsonUtility.FromJson<CodeRespone>(data);
            //servr check
            if (dataRespone.status && dataRespone.data.code_check)
            {
                // server check ok, client check
                if (dataRespone.data.mac_address.Equals(device_id) && dataRespone.data.licence_code.Equals(code))
                {
                    callback?.Invoke(dataRespone.data);
                }
                else
                {
                    onCheckOnlineFail?.Invoke();
                    //sent device_id not equals this device_id
                }
            }
            else
            {
                CodeErrorRespone errorDataResponse = JsonUtility.FromJson<CodeErrorRespone>(data);
                DbContext.Instance.Set(errorDataResponse.data.message);
                if (api == UrlConfig.CHECK_ACTIVE_API)
                    onCheckOnlineFail?.Invoke();
                else
                    errorCallback?.Invoke();
            }
        }, false,
            (status, data) =>
            {
                try
                {
                    ErrorData errorDataResponse = JsonUtility.FromJson<ErrorData>(data);
                    if (status == 0)
                    {
                        DbContext.Instance.Set("インターネット接続がありません");
                    }
                    else
                        if (errorDataResponse.message.Length > 0)
                    {
                        DbContext.Instance.Set(errorDataResponse.message);
                    }
                    else
                    {
                        Error errorResponse = JsonUtility.FromJson<Error>(data);
                        DbContext.Instance.Set(errorResponse.message);
                    }
                    errorCallback?.Invoke();
                }
                catch (Exception e)
                {
                    onTimeOutOffline?.Invoke();
                }
            });
    }

    public static IEnumerator CheckInternetConnection(Action<bool> syncResult)
    {
        string echoServer = UrlConfig.BASE_URL + UrlConfig.CHECK_ACTIVE_API;// "http://google.com";

        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
        }
        syncResult(result);
    }
}
