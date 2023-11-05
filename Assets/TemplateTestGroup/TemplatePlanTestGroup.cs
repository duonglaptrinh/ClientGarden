using Game.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TWT.Model;
using TWT.Networking;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TemplatePlanTestGroup : MonoBehaviour
{
    public static bool IsActiveSettingPlanTemplateOnEditor { get; set; } = false;
    public static bool IsTestOnEditor =>
#if UNITY_EDITOR
        true;
#else
        false;
#endif

    [SerializeField] Button btnActiveSettingPlanTemplate;

    [SerializeField] GameObject GroupSetting;
    [SerializeField] Button btnOpenFolder;
    [SerializeField] Button btnLoadbyIndex;
    [SerializeField] InputField inputFieldLoadIndex;
    [SerializeField] Button btnSaveToCurrentIndex;
    [SerializeField] Button btnSaveNewTemplatePlan;
    [SerializeField] Text textPath;
    [SerializeField] Text textName;
    string fileName = "TakashoTemplate.json";
    string fullPath => Path.Combine(Application.persistentDataPath, fileName);
    VRContentDataTemplate data = null;
    VRContentData oldDataRoom = null;
    int currentIndex = 0;
    VRObjectSync vrSync;
    // Start is called before the first frame update
    void Start()
    {
        if (IsTestOnEditor)
        {
            gameObject.SetActive(true);
            inputFieldLoadIndex.onValueChanged.AddListener(t =>
            {
                //SetTextLoadIndex();
                int.TryParse(inputFieldLoadIndex.text, out currentIndex);
                if (data != null && currentIndex >= data.vr_dome_list.Length)
                {
                    inputFieldLoadIndex.text = (data.vr_dome_list.Length - 1).ToString();
                }
            });
            btnLoadbyIndex.onClick.AddListener(OnLoadPlanByIndex);
            btnSaveToCurrentIndex.onClick.AddListener(OnSavePlanByIndex);
            btnSaveNewTemplatePlan.onClick.AddListener(OnSaveNewTemplatePlan);
            btnOpenFolder.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                Process.Start("explorer.exe", Path.GetDirectoryName(fullPath));
#endif
            });
            btnActiveSettingPlanTemplate.onClick.AddListener(() =>
            {
                IsActiveSettingPlanTemplateOnEditor = !IsActiveSettingPlanTemplateOnEditor;
                Init();
                SetTestPlanTemplate();
            });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    void Init()
    {
        GroupSetting.gameObject.SetActive(IsActiveSettingPlanTemplateOnEditor);
        if (IsActiveSettingPlanTemplateOnEditor)
        {
            vrSync = FindObjectOfType<VRObjectSync>();
            textPath.text = fullPath;
            string json = "";
            if (!File.Exists(fullPath))
            {
                DebugExtension.LogError("File not found -> Create New file." + fullPath);
                json = "{\"content_name\":\"VRG-Takasho\",\"vr_dome_list\":[{\"name\":\"house1\",\"englishName\":\"hr\",\"tag\":\"villa;apartment\",\"price\":100000,\"priceUnit\":\"¥\",\"description\":\"外構カラー:ホワイト/n敷地面積50坪以上\",\"typeHouse\":\"villa\",\"listExteriorItems\":\"De-plus門柱/nアートポートワイド庭テラスコンビ/nホームヤードループ\",\"listItemsInterior\":\"chair/nTable\",\"color\":\"(255,255,255,255)\",\"size\":\"100m2\",\"dome_id\":1,\"modelData\":{\"indexHouse\":-1,\"model_translate\":\"0,0,0\",\"model_rotation\":\"0,210.8987,0\",\"model_scale\":\"1,1,1\",\"ListHouseMaterialData\":[]},\"vr_object_list\":{\"vr_model_list\":[]},\"tranformCamera\":{\"position\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"localScale\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"eulerAngel\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}},{\"name\":\"house2\",\"englishName\":\"hr\",\"tag\":\"villa;apartment\",\"price\":100000,\"priceUnit\":\"¥\",\"description\":\"外構カラー:ホワイト/n敷地面積50坪以上\",\"typeHouse\":\"villa\",\"listExteriorItems\":\"De-plus門柱/nアートポートワイド庭テラスコンビ/nホームヤードループ\",\"listItemsInterior\":\"chair\",\"color\":\"(255,255,255,255)\",\"size\":\"40m2\",\"dome_id\":2,\"modelData\":{\"indexHouse\":-1,\"model_translate\":\"-2.44655,0.02499983,3.345137\",\"model_rotation\":\"0,223.4726,0\",\"model_scale\":\"1,1,1\",\"ListHouseMaterialData\":[]},\"vr_object_list\":{\"vr_model_list\":[]},\"tranformCamera\":{\"position\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"localScale\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"eulerAngel\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}},{\"name\":\"house3\",\"englishName\":\"hr\",\"tag\":\"villa;apartment\",\"price\":100000,\"priceUnit\":\"¥\",\"description\":\"外構カラー:ホワイト/n敷地面積50坪以上\",\"typeHouse\":\"villa\",\"listExteriorItems\":\"De-plus門柱/nアートポートワイド庭テラスコンビ/nホームヤードループ\",\"listItemsInterior\":\"Table\",\"color\":\"(255,255,255,255)\",\"size\":\"50m2\",\"dome_id\":4,\"modelData\":{\"indexHouse\":-1,\"model_translate\":\"0,0.02499989,0\",\"model_rotation\":\"0,180,0\",\"model_scale\":\"1,1,1\",\"ListHouseMaterialData\":[]},\"vr_object_list\":{\"vr_model_list\":[]},\"tranformCamera\":{\"position\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"localScale\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"eulerAngel\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}}]}";
                SaveToFile(json);
            }
            else
            {
                json = File.ReadAllText(fullPath);
                UnityEngine.Debug.LogError("Load File Template : " + fullPath);
            }
            data = JsonUtility.FromJson<VRContentDataTemplate>(json);
            SetTextLoadIndex();
        }
    }
    public void SetTestPlanTemplate()
    {
        if (IsActiveSettingPlanTemplateOnEditor)
        {
            oldDataRoom = GameContext.ContentDataCurrent;
            GameContext.ContentDataCurrent = new VRContentData(data);
        }
        else
        {
            GameContext.ContentDataCurrent = oldDataRoom;
            currentIndex = 0;
        }
        OnLoadPlanByIndex();
    }
    void SetTextLoadIndex()
    {
        inputFieldLoadIndex.text = currentIndex.ToString();
    }
    void OnLoadPlanByIndex()
    {
        VRDomeData plan = GameContext.ContentDataCurrent.vr_dome_list[currentIndex];
        vrSync.SyncDomeId(new VrArrowNextDomeMessage()
        {
            DomeId = plan.dome_id,
        });
        UnityEngine.Debug.LogError("Load Dome Id = " + plan.dome_id);
        textName.text = plan.d360_file_name;
    }
    void OnSavePlanByIndex()
    {
        VRPlanDataTemplate current = data.vr_dome_list[currentIndex];
        current.UpdateDataFromOldPlan(VrDomeControllerV2.Instance.vrDomeData);

        SaveToFile(JsonUtility.ToJson(data), () =>
        {
            Init();
            GameContext.ContentDataCurrent = new VRContentData(data);
        });
    }
    void OnSaveNewTemplatePlan()
    {
        VRPlanDataTemplate current = new VRPlanDataTemplate(data.vr_dome_list[currentIndex]);
        current.UpdateDataFromOldPlan(VrDomeControllerV2.Instance.vrDomeData);
        int max = data.vr_dome_list.Max(x => x.dome_id);
        current.dome_id = max + 1;
        data.AddNewPlan(current);

        //List<VRDomeData> list = new List<VRDomeData>();
        //list.AddRange(GameContext.ContentDataCurrent.vr_dome_list);
        //list.Add(data);
        //GameContext.ContentDataCurrent.vr_dome_list == list.ToArray();
        SaveToFile(JsonUtility.ToJson(data), () =>
        {
            Init();
            currentIndex = data.vr_dome_list.Length - 1;
            SetTextLoadIndex();
            GameContext.ContentDataCurrent = new VRContentData(data);
        });

    }
    void SaveToFile(string json, Action OnSaveDone = null)
    {
        string newData = json;
        string newPath = fullPath;
        DebugExtension.LogError(newPath);
        using (StreamWriter writer = new StreamWriter(newPath, false, Encoding.UTF8))
        {
            writer.Write(newData);
            writer.Close();
            DebugExtension.LogError("Data written to file: " + newData);
            SaveEditor(this);
            OnSaveDone?.Invoke();
        }
        //StreamWriter writer = new StreamWriter(fullPath, false);
        //writer.Write(newData);
        //writer.Close();

    }
    [ContextMenu("Save Data")]
    public static void SaveEditor(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}
