using System;
using System.Collections.Generic;
using Shared;
using Shared.NetMessages;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Client.Coins {
    public class CoinsClientService : MessageReceiverService<NetGameState>, IInitializable, IDisposable{
        private readonly ResourceLoaderService _ResourceLoaderService;
        private GameObject _CoinPrefab;
        private Dictionary<int, GameObject> _Coins = new Dictionary<int, GameObject>();
        
        public CoinsClientService(SignalBus signalBus, ResourceLoaderService resourceLoaderService) : base(signalBus) {
            _ResourceLoaderService = resourceLoaderService;
        }
        
        public void Initialize() {
            _CoinPrefab = _ResourceLoaderService.LoadResource<GameObject>("Prefabs/ClientCoin");
        }

        protected override void HandleMessage(NetGameState message) {
            for (int i = 0; i < message.CoinCount; i++) {
                var coinsState = message.CoinStates[i];
                if (!_Coins.ContainsKey(coinsState.CoinId)) {
                    var coin = SpawnPlayer(coinsState);
                    _Coins.Add(coinsState.CoinId, coin);
                }
                _Coins[coinsState.CoinId].SetActive(coinsState.IsActive);
            }
        }
        
        private GameObject SpawnPlayer(CoinState coinState) {
            var spawnPosition = new Vector3(coinState.Position.X, coinState.Position.Y, coinState.Position.Z);
            var coin = Object.Instantiate(_CoinPrefab, spawnPosition, Quaternion.identity);
            return coin;
        }

        public void Dispose() {
            SignalBus.UnSubscribeFromAll(this);
        }
    }
}