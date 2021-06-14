using System;
using Common;
using Server.Signals;
using Shared.Spawn.Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Server {
    public class BotService : IInitializable, IDisposable {
        private readonly SignalBus _SignalBus;
        private readonly UnityEventProvider _UnityEventProvider;

        private float _MoveTime = 2;
        private float _TempTime;
        private Vector2 _CurrentDirection;
        private uint _CommandNumber;
        
        public BotService(SignalBus signalBus, UnityEventProvider unityEventProvider) {
            _SignalBus = signalBus;
            _UnityEventProvider = unityEventProvider;
        }

        public void Initialize() {
            _SignalBus.Subscribe<PlayerSpawnedServerSignal>(OnServerSpawn, this);
        }

        private void OnServerSpawn(PlayerSpawnedServerSignal obj) {
            if (obj.PlayerId == 1) {
                _UnityEventProvider.OnFixedUpdate += FixedUpdate;
            }
        }

        private void FixedUpdate() {
            if (_CurrentDirection == Vector2.zero || _TempTime < 0) {
                _CurrentDirection = Random.insideUnitCircle.normalized;
                _TempTime = _MoveTime;
            }
            _SignalBus.FireSignal(new MoveSignal(1, _CommandNumber, _CurrentDirection.x, _CurrentDirection.y));
            _CommandNumber++;
            _TempTime -= Time.fixedDeltaTime;
        }

        public void Dispose() {
            _SignalBus.UnSubscribeFromAll(this);
        }
    }
}