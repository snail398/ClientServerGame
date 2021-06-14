using System;
using Common;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;

namespace Shared.Tick {
    public class TickClientService : IInitializable , IDisposable {
        private uint? _Tick;
        private readonly SignalBus _SignalBus;
        private readonly UnityEventProvider _UnityEventProvider;

        public uint? Tick => _Tick;

        public TickClientService(SignalBus signalBus, UnityEventProvider unityEventProvider) {
            _SignalBus = signalBus;
            _UnityEventProvider = unityEventProvider;
        }

        public void Initialize() {
            _SignalBus.Subscribe<StartTickClientSignal>(StartTick, this);
        }

        private void StartTick(StartTickClientSignal obj) {
            _UnityEventProvider.OnFixedUpdate += FixedUpdate;
            _Tick = new uint?();
            _Tick = 0;
            // Debug.LogError($"CLIENT::startTick");
        }

        private void FixedUpdate() {
            _Tick++;
        }

        public void Dispose() {
            _UnityEventProvider.OnFixedUpdate -= FixedUpdate;
        }
    }
}