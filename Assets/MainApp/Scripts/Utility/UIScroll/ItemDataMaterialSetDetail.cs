public class ItemDataMaterialSetDetail : ItemDataBase
{
    public string itemName;
    public MaterialData Data;
    public MaterialSet DataSet;
    public ItemDataMaterialSetDetail(string itemName, MaterialData data, MaterialSet dataSet)
    {
        this.itemName = itemName;
        Data = data;
        DataSet = dataSet;
    }
}
