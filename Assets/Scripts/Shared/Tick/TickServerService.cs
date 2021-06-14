using System;
using Common;
using UnityEngine;
using Zenject;

namespace Shared.Tick {
    public class TickServerService : IInitializable , IDisposable {
        private uint? _Tick;
        private readonly SignalBus _SignalBus;
        private readonly UnityEventProvider _UnityEventProvider;

        public uint? Tick => _Tick;

        private bool _Started;

        public TickServerService(SignalBus signalBus, UnityEventProvider unityEventProvider) {
            _SignalBus = signalBus;
            _UnityEventProvider = unityEventProvider;
        }

        public void Initialize() {
            _SignalBus.Subscribe<StartTickServerSignal>(StartTick, this);
        }

        private void StartTick(StartTickServerSignal obj) {
            if (!_Started) {
                _UnityEventProvider.OnFixedUpdate += FixedUpdate;
                _Started = true;
                _Tick = new uint?();
                _Tick = 0;
            }
            // Debug.LogError($"SERVER::startTick");
        }

        private void FixedUpdate() {
            _Tick++;
            // Debug.LogError($"SERVER::TICK::{_Tick}");
        }

        public void Dispose() {
            _UnityEventProvider.OnFixedUpdate -= FixedUpdate;
        }
    }
}