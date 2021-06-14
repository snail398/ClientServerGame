using System;
using System.Collections.Generic;
using Common;
using Server.GameFlow;
using Server.Score;
using Server.Signals;
using Shared;
using Shared.NetMessages;
using Shared.Settings;
using Shared.Spawn.Signals;
using UnityEngine;
using Zenject;

namespace Server {
    public class SynchronizationServerService : IInitializable, IDisposable {
        private readonly SignalBus _SignalBus;
        private readonly UnityEventProvider _UnityEventProvider;
        private readonly IGameStateProvider<List<CoinState>> _CoinsProvider;
        private readonly IGameStateProvider<List<PlayerState>> _PositionProvider;
        private readonly IGameStateProvider<List<ScoreState>> _ScoreProvider;
        private readonly IGameStateProvider<bool> _GameStateProvider;
        private readonly SettingsService _SettingsService;
        
        private NetworkSettings _NetworkSettings;
        private List<PlayerState> _PlayerStates = new List<PlayerState>();
        private float _TimeToTick;

        public SynchronizationServerService(SignalBus signalBus, UnityEventProvider unityEventProvider, IGameStateProvider<List<CoinState>> coinsProvider, IGameStateProvider<List<PlayerState>> positionProvider, IGameStateProvider<List<ScoreState>> scoreProvider, IGameStateProvider<bool> gameStateProvider, SettingsService settingsService) {
            _SignalBus = signalBus;
            _UnityEventProvider = unityEventProvider;
            _CoinsProvider = coinsProvider;
            _PositionProvider = positionProvider;
            _ScoreProvider = scoreProvider;
            _GameStateProvider = gameStateProvider;
            _SettingsService = settingsService;
        }

        public void Initialize() {
             _SignalBus.Subscribe<PlayerSpawnedServerSignal>(OnPlayerSpawn, this);   
             _SignalBus.Subscribe<ForceSyncSignal>(ForceSync, this);   
             _SignalBus.Subscribe<StartGameSignal>(OnGameStart, this);   
             _NetworkSettings = _SettingsService.GetSettings<NetworkSettings>();
             _TimeToTick = (float)1 / _NetworkSettings.Settings.ServerUpdatePerSeconds;
        }

        private void OnGameStart(StartGameSignal obj) {
            _UnityEventProvider.OnUpdate += Update;
        }

        private void OnPlayerSpawn(PlayerSpawnedServerSignal obj) {
            var playerState = new PlayerState();
            playerState.PlayerId = obj.PlayerId;
            _PlayerStates.Add(playerState);
        }

        private void Update() {
            _TimeToTick -= Time.deltaTime;
            if (_TimeToTick <= 0) {
                _TimeToTick = (float)1 / _NetworkSettings.Settings.ServerUpdatePerSeconds;
                Sync();
            }
        }

        private void Sync() {
            if (_PlayerStates.Count == 0)
                return;
            _SignalBus.FireSignal(new UpdateGameStateSignal());
            _SignalBus.FireSignal(new BroadcastMessageSignal(new NetGameState(_GameStateProvider.State, _PositionProvider.State, _CoinsProvider.State, _ScoreProvider.State)));
        }
        
        private void ForceSync(ForceSyncSignal obj) {
            Sync();
        }

        public void Dispose() {
            _UnityEventProvider.OnUpdate -= Update;
        }
    }
}