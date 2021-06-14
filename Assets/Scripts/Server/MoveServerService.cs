using System;
using System.Collections.Generic;
using Common;
using Server.Signals;
using Shared;
using Shared.NetMessages;
using Shared.Spawn.Signals;
using UnityEngine;
using Zenject;

namespace Server {

    public class PlayerInfo {
        public PlayerView Player;
        public readonly Vector2[] ClientCommandBuffer = new Vector2[1024];
        public uint LastHandledCommandNumber;
        public uint LastReceivedCommandNumber;
    }
    
    public class MoveServerService : MessageReceiverService<NetInputMessage>, IInitializable, IDisposable, IGameStateProvider<List<PlayerState>> {

        private readonly Dictionary<int, PlayerInfo> _Players = new Dictionary<int, PlayerInfo>();
        private List<PlayerState> _PlayerStates = new List<PlayerState>();
        
        public MoveServerService(SignalBus signalBus) : base(signalBus) { }

        protected override void HandleMessage(NetInputMessage message) {
            HandlePlayerInput(message.PlayerId, message.CommandNumber, new Vector2(message.DirectionX, message.DirectionY));
        }

        private void HandlePlayerInput(int playerId, uint commandNumber, Vector2 moveDirection) {
            // Clear missed command in command buffer
            var playerInfo = _Players[playerId];
            for (var i = playerInfo.LastReceivedCommandNumber + 1; i < commandNumber; i++) {
                var index = i % 1024;
                playerInfo.ClientCommandBuffer[index] = Vector2.zero;
            }
            playerInfo.ClientCommandBuffer[commandNumber % 1024] = moveDirection;
            playerInfo.LastReceivedCommandNumber = commandNumber;
        }
        
        public void Initialize() {
            SignalBus.Subscribe<PlayerSpawnedServerSignal>(OnPlayerSpawned, this);
            SignalBus.Subscribe<UpdateGameStateSignal>(UpdateGameState, this);
            SignalBus.Subscribe<MoveSignal>(OnMoveSignal, this);
        }

        private void OnMoveSignal(MoveSignal obj) {
            HandlePlayerInput(obj.PlayerId, obj.CommandNumber, new Vector2(obj.DirectionX, obj.DirectionY));
        }

        private void UpdateGameState(UpdateGameStateSignal obj) {
            foreach (var kvp in _Players) {
                var playerInfo = kvp.Value;
                for (var i = playerInfo.LastHandledCommandNumber + 1; i <= playerInfo.LastReceivedCommandNumber; i++) {
                    var index = i % 1024;
                    playerInfo.Player.transform.position += (new Vector3(playerInfo.ClientCommandBuffer[index].x, 0, playerInfo.ClientCommandBuffer[index].y)) * Time.fixedDeltaTime;
                }
                playerInfo.LastHandledCommandNumber = playerInfo.LastReceivedCommandNumber;
            }
        }

        private void OnPlayerSpawned(PlayerSpawnedServerSignal obj) {
            var playerInfo = new PlayerInfo();
            playerInfo.Player = obj.Player;
            _Players.Add(obj.PlayerId, playerInfo);
            _PlayerStates.Add(new PlayerState(obj.PlayerId));
        }

        public void Dispose() {
            SignalBus.UnSubscribeFromAll(this);
        }

        public List<PlayerState> State {
            get {
                for (int i = 0; i < _PlayerStates.Count; i++) {
                    var playerState = _PlayerStates[i];
                    var playerInfo = _Players[playerState.PlayerId];
                    playerState.LastHandledCommand = playerInfo.LastHandledCommandNumber;
                    var pos = playerInfo.Player.transform.position;
                    playerState.Position.X = pos.x;
                    playerState.Position.Y = pos.y;
                    playerState.Position.Z = pos.z;
                }

                return _PlayerStates;
            }
        }
    }
}