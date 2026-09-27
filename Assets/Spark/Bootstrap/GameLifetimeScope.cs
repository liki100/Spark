using Spark.Installer;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Spark.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private TicTacToeInstaller _ticTacToeInstaller;
        [SerializeField] private ModalInstaller _modalInstaller;

        protected override void Configure(IContainerBuilder builder)
        {
            _modalInstaller.Install(builder);
            _ticTacToeInstaller.Install(builder);
        }
    }
}
