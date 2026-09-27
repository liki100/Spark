using Spark.Inventory.Content;
using Spark.Inventory.Model;

namespace Spark.Inventory.Installer
{
    public class MaxStackProvider : IMaxStackProvider
    {
        private readonly ItemLibrary _library;
        
        public MaxStackProvider(ItemLibrary library)
        {
            _library = library;
        }
        
        public int GetMaxStack(string id) => _library.TryGet(id, out var definition) ? definition.MaxStack : 0;
    }
}