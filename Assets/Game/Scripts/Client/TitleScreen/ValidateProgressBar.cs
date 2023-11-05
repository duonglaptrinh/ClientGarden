using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWT.Client.ContentScreen;
using Game.Client;

public class ValidateProgressBar : MonoBehaviour
{
    LoadingDataView loadingDataView;
    void Awake()
    {
        loadingDataView = GetComponent<LoadingDataView>();
    }

    void OnEnable()
    {
        loadingDataView.UpdateProcess(0f);
    }

    void Update()
    {
        if(VrResourceStruct.Newest != null)
        {
            float validateProgress = VrResourceStruct.Newest.ValidateProgress;
            float delta = (validateProgress - loadingDataView.Value) / 10f;
            loadingDataView.UpdateProcess(Mathf.Clamp(loadingDataView.Value + (delta < 0.02f ? 0.02f : delta), 0, validateProgress));
        }
    }
}
