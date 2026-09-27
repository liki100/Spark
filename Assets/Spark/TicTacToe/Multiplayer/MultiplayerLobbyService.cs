using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;

namespace Spark.TicTacToe.Multiplayer
{
    public class MultiplayerLobbyService
    {
        public ISession CurrentSession { get; private set; }
        public bool IsHost => CurrentSession?.IsHost ?? false;
        
        public event Action<string> PlayerHasLeft;

        public async UniTask InitializeAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async UniTask<ISession> QuickJoinAsync()
        {
            var quickJoinOptions = new QuickJoinOptions
            {
                Timeout = TimeSpan.FromSeconds(5),
                CreateSession = false
            };
            
            CurrentSession = await MultiplayerService.Instance.MatchmakeSessionAsync(quickJoinOptions, null);
            CurrentSession.PlayerHasLeft += OnPlayerHasLeft;
            return CurrentSession;
        }

        public async UniTask<ISession> CreateSessionAsync(SessionOptions sessionOptions)
        {
            CurrentSession = await MultiplayerService.Instance.CreateSessionAsync(sessionOptions);
            CurrentSession.PlayerHasLeft += OnPlayerHasLeft;
            return CurrentSession;
        }

        public async UniTask<ISession> JoinSessionByCodeAsync(string joinCode)
        {
            CurrentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            CurrentSession.PlayerHasLeft += OnPlayerHasLeft;
            return CurrentSession;
        }

        public async UniTask<ISession> JoinSessionByIdAsync(string sessionId)
        {
            CurrentSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
            CurrentSession.PlayerHasLeft += OnPlayerHasLeft;
            return CurrentSession;
        }

        public async UniTask LeaveSessionAsync()
        {
            CurrentSession.PlayerHasLeft -= OnPlayerHasLeft;
            await CurrentSession.LeaveAsync();
        }
        
        public async UniTask<IList<ISessionInfo>> GetSessionsAsync()
        {
            var querySessionsOptions = new QuerySessionsOptions();
            var results = await MultiplayerService.Instance.QuerySessionsAsync(querySessionsOptions);
            return results.Sessions;
        }
        
        private void OnPlayerHasLeft(string playerId) => PlayerHasLeft?.Invoke(playerId);
    }
}