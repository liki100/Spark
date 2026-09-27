using Spark.Save.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Spark.Save.Installer
{
    public class SaveInstaller : MonoBehaviour, IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<SaveService>(Lifetime.Singleton);
        }
    }
}