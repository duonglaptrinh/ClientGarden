using Game.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TWT.Networking;
using UnityEngine;

namespace TWT.Model
{
    [Serializable]
    public class VRListStartPointData
    {
        public int indexStartPoint = -1;
        public List<VRStartPointData> listStartPoint;
        public VRStartPointData CurrentData
        {
            get
            {
                if (indexStartPoint < 0 || indexStartPoint >= listStartPoint.Count) return null;
                return listStartPoint[indexStartPoint];
            }
        }
        public VRListStartPointData()
        {
            indexStartPoint = -1;
            listStartPoint = new List<VRStartPointData>();
        }
        public VRListStartPointData(VRListStartPointData oldData)
        {
            this.indexStartPoint = oldData.indexStartPoint;
            List<VRStartPointData> list = new List<VRStartPointData>();
            foreach (VRStartPointData data in oldData.listStartPoint)
            {
                list.Add(new VRStartPointData(data));
            }
            this.listStartPoint = list;
        }
        public void AddData(VRStartPointData data)
        {
            listStartPoint.Add(data);
        }
        public void DeleteData(int index, VRStartPointData data)
        {
            if (index < 0 || index >= listStartPoint.Count) return;
            listStartPoint.RemoveAt(index);
        }
        public void UpdateData(int index, VRStartPointData data)
        {
            if (index < 0 || index >= listStartPoint.Count) return;
            listStartPoint[index] = data;
        }
        public void SetCurrentIndexStartPoint(int newIndex)
        {
            this.indexStartPoint = newIndex;
        }
        /// <summary>
        /// Send data to sync
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="indexInList">only use for update, Delete and view</param>
        public static void SendDataSync(EStartPoint type, VRStartPointData data, int indexInList = -1)
        {
            string json = JsonUtility.ToJson(data);
            //DebugExtension.LogError("Send Data VRStartPointData: type = " + type.ToString() + "     value = " + json);

            //Test 
            //return;
            //End Test

            VrgSyncApi.Send(new SyncStartPointMessage()
            {
                idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
                type = (int)type,
                json = json,
                indexInList = indexInList
            }, SyncStartPointMessage.EventKey);
        }
    }
    [Serializable]
    public class VRStartPointData : VRTransformData
    {
        public string nameView;
        public string id_url;
        public VRStartPointData() : base()
        {
            nameView = string.Empty;
            this.id_url = string.Empty;
        }
        public VRStartPointData(VRStartPointData oldData) : base(oldData)
        {
            nameView = oldData.nameView;
            this.id_url = oldData.id_url;
        }
        public static VRStartPointData CreateStart()
        {
            VRStartPointData data = new VRStartPointData();
            data.nameView = "startView";
            data.id_url = "-1";
            data.position = new Vector3(-3.5f, 0, 7f);
            data.localScale = new Vector3(1, 1, 1);
            data.eulerAngel = new Vector3(0, 180, 0);
            data.rotation = new Quaternion(0, 1, 0, 0);
            return data;
        }
    }

}