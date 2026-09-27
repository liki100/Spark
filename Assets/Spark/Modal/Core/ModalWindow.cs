using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spark.Modal.Core
{
    public abstract class ModalWindow<TArgs, TResult> : MonoBehaviour
    {
        public abstract UniTask<ModalResult<TResult>> OpenAsync(TArgs args, CancellationToken cancellationToken);
    }
}
