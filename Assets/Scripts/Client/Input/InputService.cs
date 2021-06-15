using System;
using Client.Signals;
using Client.Spawn;
using Common;
using Shared.NetMessages;
using Shared.Tick;
using UnityEngine;
using Zenject;

namespace Client.Input {
    public class InputService : IInitializable, IDisposable {
        private readonly UnityEventProvider _UnityEventProvider;
        private readonly SignalBus _SignalBus;

        public Vector2 MoveDirection;
        private uint _CommandNumber;
        private Vector2[] _CommandBuffer = new Vector2[1024];
        private int _LocalId;

        public Vector2[] CommandBuffer => _CommandBuffer;

        public uint CommandNumber => _CommandNumber;

        public InputService(UnityEventProvider unityEventProvider, SignalBus signalBus) {
            _UnityEventProvider = unityEventProvider;
            _SignalBus = signalBus;
        }

        private void OnPlayerSpawn(PlayerSpawnClientSignal obj) {
            if (obj.IsLocal) {
                _LocalId = obj.PlayerId;
                _UnityEventProvider.OnUpdate += Update;
                _UnityEventProvider.OnFixedUpdate += FixedUpdate;
            }
        }

        public void Initialize() {
            _SignalBus.Subscribe<PlayerSpawnClientSignal>(OnPlayerSpawn, this);
        }
        
        private void Update() {
            var hor = UnityEngine.Input.GetAxis("Horizontal");
            var vert = UnityEngine.Input.GetAxis("Vertical");
            MoveDirection = new Vector2(hor, vert);
        }

        private void FixedUpdate() {
            _SignalBus.FireSignal(new SendMessageToServerSignal(new NetInputMessage(_LocalId, _CommandNumber, MoveDirection.x, MoveDirection.y)));
            _SignalBus.FireSignal(new ApplyLocalInputSignal(MoveDirection.x, MoveDirection.y));
            _CommandBuffer[_CommandNumber % 1024] = MoveDirection;
            _CommandNumber++;
        }

        public void Dispose() {
            _UnityEventProvider.OnUpdate -= Update;
            _UnityEventProvider.OnFixedUpdate -= FixedUpdate;
        }
    }
}