using System;
using System.Collections;
using System.Collections.Generic;
using Client.Input;
using Client.Signals;
using Client.Spawn;
using Common;
using Shared;
using Shared.NetMessages;
using UnityEngine;
using Zenject;

namespace Client {
    public class MoveClientService : MessageReceiverService<NetGameState>, IInitializable, IDisposable {

        private readonly InputService _InputService;
        private readonly ICoroutineExecutor _CoroutineExecutor;
        
        private readonly Dictionary<int, GameObject> _Players = new Dictionary<int, GameObject>();
        private GameObject _LocalPlayer;
        private int _LocalPlayerId = 0;
        private readonly Dictionary<int, Vector3[]> _PlayersStateBuffers = new Dictionary<int, Vector3[]>();
        private readonly Dictionary<int, Coroutine> _InterpolateCoroutines = new Dictionary<int, Coroutine>();
        private uint _LastReceivedMessageCount;
        
        public MoveClientService(SignalBus signalBus, InputService inputService, ICoroutineExecutor coroutineExecutor) : base(signalBus) {
            _InputService = inputService;
            _CoroutineExecutor = coroutineExecutor;
        }
        
        protected override void HandleMessage(NetGameState message) {
            _LastReceivedMessageCount++;
            for (int i = 0; i < message.PlayerCount; i++) {
                var playerState = message.PlayerStates[i];
                if (playerState.PlayerId == message.LocalId) {
                    LocalPlayerMove(playerState);
                }
                else {
                    RemotePlayerMove(playerState);
                }
            }
        }

        private void RemotePlayerMove(PlayerState playerState) {
            var playerBuffer = _PlayersStateBuffers[playerState.PlayerId];
            var serverPosition = new Vector3(playerState.Position.X, playerState.Position.Y, playerState.Position.Z);
            playerBuffer[_LastReceivedMessageCount % 1024] = serverPosition;
            var previousIndex = _LastReceivedMessageCount % 1024 == 0 ? 1023 : (_LastReceivedMessageCount % 1024) - 1;
            var previousPosition = playerBuffer[previousIndex];
            var player = _Players[playerState.PlayerId];

            if (_InterpolateCoroutines.TryGetValue(playerState.PlayerId, out var coroutine) ) {
                _CoroutineExecutor.StopCoroutine(coroutine);
                _InterpolateCoroutines[playerState.PlayerId] = null;
            }
            _InterpolateCoroutines[playerState.PlayerId] = _CoroutineExecutor.StartCoroutine(InterpolatePosition(player, previousPosition, serverPosition));
        }

        private IEnumerator InterpolatePosition(GameObject player, Vector3 previousPosition, Vector3 serverPosition) {
            player.transform.position = previousPosition;
            var timeToInterpolate = (float) 1 / 20;
            var step = (serverPosition - previousPosition).magnitude * Time.fixedDeltaTime / timeToInterpolate;
            while (timeToInterpolate > 0) {
                timeToInterpolate -= Time.deltaTime;
                player.transform.position += (serverPosition - previousPosition).normalized * step;
                yield return new WaitForFixedUpdate();
            }
            player.transform.position = serverPosition;
        }

        private void LocalPlayerMove(PlayerState playerState) {
            //server reconstillation
            var serverPosition = new Vector3(playerState.Position.X, playerState.Position.Y, playerState.Position.Z);
            var player = _Players[playerState.PlayerId];
            player.transform.position = serverPosition;
            for (var j = playerState.LastHandledCommand + 1; j < _InputService.CommandNumber; j++) {
                var index = j % 1024;
                player.transform.position +=
                    new Vector3(_InputService.CommandBuffer[index].x, 0, _InputService.CommandBuffer[index].y) *
                    Time.fixedDeltaTime;
            }
        }

        public void Initialize() {
            SignalBus.Subscribe<PlayerSpawnClientSignal>(OnPlayerSpawned, this);
            SignalBus.Subscribe<ApplyLocalInputSignal>(OnLocalInput, this);
        }

        private void OnLocalInput(ApplyLocalInputSignal obj) {
            //move prediction
            _LocalPlayer.transform.position += new Vector3(obj.DirectionX, 0, obj.DirectionY) * Time.fixedDeltaTime;
        }

        private void OnPlayerSpawned(PlayerSpawnClientSignal obj) {
            _Players.Add(obj.PlayerId, obj.Player);
            _PlayersStateBuffers.Add(obj.PlayerId, new Vector3[1024]);
            if (obj.IsLocal) {
                _LocalPlayer = obj.Player;
            }
        }

        public void Dispose() {
            SignalBus.UnSubscribeFromAll(this);
        }
    }
}