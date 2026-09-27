namespace Spark.Inventory.Model
{
    public interface IMaxStackProvider
    {
        int GetMaxStack(string id);
    }
}