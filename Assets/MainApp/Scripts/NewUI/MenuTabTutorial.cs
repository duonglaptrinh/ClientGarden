using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabTutorial : MonoBehaviour
{
    [SerializeField] protected Image icon;
    [SerializeField] protected Text textName;
    //[SerializeField] protected Text textTime;
    [SerializeField] protected Text textPage;
    [SerializeField] protected Text textContent;
    [SerializeField] protected Button btnClose;
    [SerializeField] protected Button btnNext;
    [SerializeField] protected Button btnPre;
    int currentContent = 0;
    TutorialData currentDataContent;
    // Start is called before the first frame update
    void Start()
    {
        btnClose.onClick.AddListener(OnClose);
        btnNext.onClick.AddListener(OnNext);
        btnPre.onClick.AddListener(OnPrev);
    }
    public void LoadData(TutorialData dataContent)
    {
        currentContent = 0;
        currentDataContent = dataContent;
        LoadData();
    }
    void LoadData()
    {
        TutorialContentData data = currentDataContent.content_list[currentContent];
        textName.text = currentDataContent.name;
        icon.sprite = LoadResourcesData.Instance.GetSpriteBackgroundTutorialByName(data.image_id);
        textPage.text = (currentContent + 1) + " / " + currentDataContent.content_list.Count;
        textContent.text = data.text;
        //textTime.text = dataContent.time;
    }
    void OnClose() { }
    void OnNext()
    {
        currentContent++;
        if (currentContent >= currentDataContent.content_list.Count) currentContent = 0;
        LoadData();
    }
    private void OnPrev()
    {
        currentContent--;
        if (currentContent < 0) currentContent = currentDataContent.content_list.Count - 1;
        LoadData();
    }
}
