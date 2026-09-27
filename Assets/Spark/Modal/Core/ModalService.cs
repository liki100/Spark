using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Spark.Modal.Core
{
    public class ModalService : IDisposable
    {
        private readonly Transform _root;
        private readonly Dictionary<Type, string> _registrations = new();
        private readonly Dictionary<Type, AsyncOperationHandle<GameObject>> _handles = new();

        public ModalService(ModalRoot root)
        {
            _root = root.transform;
        }
        
        public ModalService Register<TModal>(string address)
        {
            _registrations[typeof(TModal)] = address;
            return this;
        }
        
        public UniTask<ModalResult<TResult>> OpenAsync<TModal, TResult>()
            where TModal : ModalWindow<Unit, TResult>
        {
            return OpenAsync<TModal, Unit, TResult>(Unit.Default);
        }
        
        public async UniTask<ModalResult<TResult>> OpenAsync<TModal, TArgs, TResult>(TArgs args)
            where TModal : ModalWindow<TArgs, TResult>
        {
            var prefab = await GetPrefabAsync<TModal>();
            var instance = UnityEngine.Object.Instantiate(prefab, _root);
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var modal = instance.GetComponent<TModal>();

                if (modal == null)
                    throw new InvalidOperationException($"Prefab registered for {typeof(TModal).Name} must have a {typeof(TModal).Name} component.");

                return await modal.OpenAsync(args, cancellationTokenSource.Token);
            }
            finally
            {
                cancellationTokenSource.Dispose();
                UnityEngine.Object.Destroy(instance);
            }
        }
        
        private UniTask<GameObject> GetPrefabAsync<TModal>()
        {
            var type = typeof(TModal);

            if (_handles.TryGetValue(type, out var cachedHandle))
                return cachedHandle.ToUniTask();

            if (!_registrations.TryGetValue(type, out var address))
                throw new InvalidOperationException($"Modal window {type.Name} is not registered.");

            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            _handles.Add(type, handle);

            return handle.ToUniTask();
        }
        
        public void Dispose()
        {
            foreach (var handle in _handles.Values)
                Addressables.Release(handle);

            _handles.Clear();
        }
    }
}
