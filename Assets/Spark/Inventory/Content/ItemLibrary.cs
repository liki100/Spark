using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Spark.Inventory.Content
{
    public class ItemLibrary
    {
        private readonly Dictionary<string, ItemDefinition> _definitionMap = new();
        
        public async UniTask LoadAsync()
        {
            var definitions = await Addressables.LoadAssetsAsync<ItemDefinition>("items").ToUniTask();
            foreach (var definition in definitions) 
                _definitionMap[definition.Id] = definition;
        }
        
        public bool TryGet(string id, out ItemDefinition definition) => _definitionMap.TryGetValue(id, out definition);
    }
}