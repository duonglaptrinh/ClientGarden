using System;
using System.Collections.Generic;
using System.Linq;
using TWT.Model;

class BaseScreenDataManager
{
    public static Action OnDeleteDomeData = null;
    public static Action OnChangeDome = null;
    public static VRDomeData GetDomeById(int vrDomeId)
    {
        if (vrDomeId == -1)
            return null;

        VRDomeData dome = GameContext.ContentDataCurrent.vr_dome_list.Where(item => item.dome_id == vrDomeId).First();
        return dome;
    }

    public static void AddNewDome(VRDomeData vRDomeData)
    {
        if (GameContext.ContentDataCurrent == null)
            return;

        List<VRDomeData> listDomes = new List<VRDomeData>(GameContext.ContentDataCurrent.vr_dome_list);
        listDomes.Add(vRDomeData);
        GameContext.ContentDataCurrent.vr_dome_list = listDomes.ToArray();
    }

    public static void UpdateDome(VRDomeData vRDomeData)
    {
        if (GameContext.ContentDataCurrent == null)
            return;

        if (GameContext.ContentDataCurrent.vr_dome_list.Where(item => item.dome_id == vRDomeData.dome_id).Any())
        {
            VRDomeData dome = GameContext.ContentDataCurrent.vr_dome_list.Where(item => item.dome_id == vRDomeData.dome_id).First();
            dome = vRDomeData;
        }
    }

    public static void DeleteDome(int deletedDomeId)
    {
        if (GameContext.ContentDataCurrent == null)
            return;

        GameContext.ContentDataCurrent.vr_dome_list = GameContext.ContentDataCurrent.vr_dome_list.Where(item => item.dome_id != deletedDomeId).ToArray();
        OnDeleteDomeData?.Invoke();
    }

    public static void SyncDomeById(int domeId)
    {
        OnChangeDome?.Invoke();
    }

}

