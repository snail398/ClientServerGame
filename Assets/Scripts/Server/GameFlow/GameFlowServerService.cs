using System;
using System.Collections.Generic;
using Client.Coins;
using Client.UI;
using Server.Score;
using Server.Signals;
using Zenject;

namespace Server.GameFlow {
    public class GameFlowServerService : IInitializable, IDisposable, IGameStateProvider<bool> {
        private readonly SignalBus _SignalBus;
        private readonly ScoreServerService _ScoreServerService;
        private readonly List<int> _ConnectedClients = new List<int>();
        private bool _GameActive;

        public List<int> ConnectedClients => _ConnectedClients;
        public bool GameActive => _GameActive;
        public bool State => _GameActive;

        public GameFlowServerService(SignalBus signalBus, ScoreServerService scoreServerService) {
            _SignalBus = signalBus;
            _ScoreServerService = scoreServerService;
        }

        public void Initialize() {
            _SignalBus.Subscribe<ClientConnectedSignal>(OnClientConnected, this);
            _SignalBus.Subscribe<CheckForEndGameSignal>(CheckForEndGame, this);
        }

        private void CheckForEndGame(CheckForEndGameSignal obj) {
            if (_ScoreServerService.HasWinner()) {
                StopGame();
            }
            else {
                _SignalBus.FireSignal(new SpawnCoinSignal());
            }
        }

        private void OnClientConnected(ClientConnectedSignal obj) {
            _ConnectedClients.Add(obj.Id);
            if (!_GameActive && _ConnectedClients.Count > 1) {
               StartGame();
            }
        }

        private void StartGame() {
            _GameActive = true;
            _SignalBus.FireSignal(new StartGameSignal());
        }

        private void StopGame() {
            _GameActive = false;
            _SignalBus.FireSignal(new StopGameSignal());
        }

        public void Dispose() {
            _SignalBus.UnSubscribeFromAll(this);
        }
    }
}