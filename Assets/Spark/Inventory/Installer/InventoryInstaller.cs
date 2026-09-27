using Spark.Inventory.Model;
using Spark.Inventory.View;
using Spark.Inventory.ViewModel;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Spark.Inventory.Installer
{
    public class InventoryInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private int _defaultCapacity = 12;
        
        public void Install(IContainerBuilder builder)
        {
            builder.Register<MaxStackProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InventoryModel>(Lifetime.Singleton).WithParameter<int>(_defaultCapacity);
            builder.Register<InventoryViewModel>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterComponent(_inventoryView);
        }
    }
}