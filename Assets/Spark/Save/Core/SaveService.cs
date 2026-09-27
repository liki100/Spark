using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VContainer.Unity;

namespace Spark.Save.Core
{
    public class SaveService : IStartable, IDisposable
    {
        private const int DELAY_SECONDS = 10;

        private readonly Dictionary<string, ISaveEntry> _registrations = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private JObject _root;
        private bool _isDirty;
        
        private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public void Start() => AutoSaveLoopAsync(_cancellationTokenSource.Token).Forget();
        
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        public void Register<TState>(string key, ISaveParticipant<TState> participant)
        {
            var entry = new SaveEntry<TState>(participant);
            if (!_registrations.TryAdd(key, entry))
                Debug.LogWarning($"Registration for key {key} already exists");
            
            Restore(key, entry);
        }
        
        public async UniTask FetchSnapshotAsync(CancellationToken cancellationToken = default)
        {
            if (File.Exists(SavePath) == false)
                return;
            
            try
            {
                var decrypted = await File.ReadAllBytesAsync(SavePath, cancellationToken).AsUniTask();
                var json = Encoding.UTF8.GetString(XorCipher.Apply(decrypted));
                _root = JObject.Parse(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        
        public void SetDirty() => _isDirty = true;

        public async UniTask SaveAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _root = new JObject();
                foreach (var pair in _registrations)
                    _root[pair.Key] = pair.Value.CaptureToken();

                var json = _root.ToString();
                var encrypted = XorCipher.Apply(Encoding.UTF8.GetBytes(json));
                await File.WriteAllBytesAsync(SavePath, encrypted, cancellationToken).AsUniTask();
                _isDirty = false;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public void Load()
        {
            foreach (var pair in _registrations)
                Restore(pair.Key, pair.Value);
        }

        private void Restore(string key, ISaveEntry entry)
        {
            if (_root == null) return;
            
            try
            {
                var token = _root[key];
                if (token != null)
                    entry.RestoreToken(token);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        private async UniTask AutoSaveLoopAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(DELAY_SECONDS), cancellationToken: cancellationToken);

                if (_isDirty)
                    await SaveAsync(cancellationToken);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}