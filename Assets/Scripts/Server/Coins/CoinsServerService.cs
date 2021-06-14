using System;
using System.Collections.Generic;
using Client.Coins;
using Server.GameFlow;
using Server.Score;
using Shared;
using Shared.Coins;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Server.Coins {
    public class CoinsServerService : IInitializable, IDisposable, IGameStateProvider<List<CoinState>> {
        private readonly SignalBus _SignalBus;
        private readonly ResourceLoaderService _ResourceLoaderService;

        private CoinView _CoinPrefab;
        private readonly List<CoinState> _CoinStates = new List<CoinState>();
        private readonly Dictionary<int, Coin> _Coins = new Dictionary<int, Coin>();
        private int _TempId;
        private int _FreeCoin;

        public CoinsServerService(SignalBus signalBus, ResourceLoaderService resourceLoaderService) {
            _SignalBus = signalBus;
            _ResourceLoaderService = resourceLoaderService;
        }

        public List<CoinState> State {
            get {
                for (int i = 0; i < _CoinStates.Count; i++) {
                    var coin = _Coins[i];
                    _CoinStates[i].Position = coin.Position;
                    _CoinStates[i].IsActive = coin.IsActive;
                }

                return _CoinStates;
            }
        }

        public void Initialize() {
            _CoinPrefab = _ResourceLoaderService.LoadResource<CoinView>("Prefabs/ServerCoin");
            _SignalBus.Subscribe<StartGameSignal>(OnGameStart, this);
            _SignalBus.Subscribe<SpawnCoinSignal>(OnSpawnCoin, this);
        }

        private void OnSpawnCoin(SpawnCoinSignal obj) {
            CreateCoin();
        }

        private void OnGameStart(StartGameSignal obj) {
            for (int i = 0; i < 10; i++) {
                CreateCoin();
            }
            _SignalBus.FireSignal(new ForceSyncSignal());
        }

        private void CreateCoin() {
            var coinView = SpawnCoin();
            coinView.OnTake += OnCoinTake;
            coinView.Init(_TempId);
            _Coins.Add(_TempId, new Coin(coinView, _TempId));
            _CoinStates.Add(new CoinState(_TempId));
            _TempId++;
            _FreeCoin++;
        }

        private void OnCoinTake(int coinId, int playerId) {
            _Coins[coinId].IsActive = false;
            _SignalBus.FireSignal(new TakeCoinSignal(playerId));
            _FreeCoin--;
            if (_FreeCoin == 0) {
                _SignalBus.FireSignal(new CheckForEndGameSignal());
            }
        }

        private CoinView SpawnCoin() {
            var spawnPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
            var coinView = Object.Instantiate(_CoinPrefab, spawnPosition, Quaternion.identity);
            return coinView;
        }
        
        public void Dispose() {
            _SignalBus.UnSubscribeFromAll(this);
        }

    }
}