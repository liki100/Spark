using Spark.Modal.Core;
using Spark.TicTacToe.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Spark.Installer
{
    public class ModalInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private ModalRoot _modalRoot;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(_modalRoot);
            builder.Register<ModalService>(Lifetime.Singleton);
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Resolve<ModalService>()
                    .Register<CreateSessionModal>("CreateSessionModal");
            });
        }
    }
}
