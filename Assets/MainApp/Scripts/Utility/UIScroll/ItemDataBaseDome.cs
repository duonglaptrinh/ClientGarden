using TWT.Model;

public class ItemDataBaseDome : ItemDataBase
{
    public VRDomeData domeData;
    public bool IsCreateNew;

    public VRPlanDataTemplate planDataTemplate;
    /// <summary>
    /// Only Use for Save Menu
    /// </summary>
    /// <param name="domeData"></param>
    public ItemDataBaseDome(VRDomeData domeData, bool isCreateNew)
    {
        this.domeData = domeData;
        this.IsCreateNew = isCreateNew;
    }
    /// <summary>
    /// Only Use for PlanMenu - Template plan
    /// </summary>
    /// <param name="domeData"></param>
    public ItemDataBaseDome(VRPlanDataTemplate domeData)
    {
        this.planDataTemplate = domeData;
    }
}
