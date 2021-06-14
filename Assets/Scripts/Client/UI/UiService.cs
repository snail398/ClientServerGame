using System;
using System.Collections.Generic;
using Client.GameFlow;
using Client.Spawn;
using Server.GameFlow;
using Shared;
using Shared.NetMessages;
using Zenject;
using Object = UnityEngine.Object;

namespace Client.UI {
    public class UiService : MessageReceiverService<NetGameState>, IInitializable, IDisposable {
        private readonly ScoreContainer _ScoreContainer;
        private readonly InfoPanel _InfoPanel;
        private readonly ResourceLoaderService _ResourceLoaderService;
        
        private readonly Dictionary<int, ScoreItem> _ScoreItems = new Dictionary<int, ScoreItem>();
        private ScoreItem _ScoreItemPrefab;

        public UiService(SignalBus signalBus, ScoreContainer scoreContainer, ResourceLoaderService resourceLoaderService, InfoPanel infoPanel) : base(signalBus) {
            _ScoreContainer = scoreContainer;
            _ResourceLoaderService = resourceLoaderService;
            _InfoPanel = infoPanel;
        }

        public void Initialize() {
            SignalBus.Subscribe<PlayerSpawnClientSignal>(OnPlayerSpawned, this);
            SignalBus.Subscribe<StartGameClientSignal>(OnGameStarted, this);
            SignalBus.Subscribe<ShowWinnerSignal>(OnShowWinner, this);
            _ScoreItemPrefab = _ResourceLoaderService.LoadResource<ScoreItem>("Prefabs/UI/ScoreItem");
            _InfoPanel.ShowText("WAITING FOR CONNECTION OTHER PLAYER", 3);
        }

        private void OnGameStarted(StartGameClientSignal obj) {
            _InfoPanel.ShowText("GAME STARTED!!!", 3);
        }

        private void OnShowWinner(ShowWinnerSignal obj) {
            _InfoPanel.ShowText($"WINNER - Player {obj.PlayerId}", 3);
        }

        private void OnPlayerSpawned(PlayerSpawnClientSignal obj) {
            var scoreItem = Object.Instantiate(_ScoreItemPrefab, _ScoreContainer.transform);
            scoreItem.Init(obj.PlayerId, 0);
            _ScoreItems.Add(obj.PlayerId, scoreItem);
        }
        
        protected override void HandleMessage(NetGameState message) {
            for (int i = 0; i < message.PlayerCount; i++) {
                var scoreState = message.ScoreStates[i];
                _ScoreItems[scoreState.PlayerId].SetScore(scoreState.Score);
            }
        }
        
        public void Dispose() {
            SignalBus.UnSubscribeFromAll(this);
        }
    }
}