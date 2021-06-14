using System;
using System.Collections.Generic;
using Client.UI;
using Server.Score;
using Shared;
using Shared.NetMessages;
using Zenject;

namespace Client.GameFlow {
    public class GameFlowClientService : MessageReceiverService<NetGameState>, IInitializable, IDisposable {
        private bool _GameActive;
        
        public GameFlowClientService(SignalBus signalBus) : base(signalBus) {
        }

        protected override void HandleMessage(NetGameState message) {
            if (!_GameActive && message.GameActive) {
                _GameActive = true;
                SignalBus.FireSignal(new StartGameClientSignal());
            }
            if (_GameActive && !message.GameActive) {
                _GameActive = false;
                SignalBus.FireSignal(new StopGameClientSignal());
                SignalBus.FireSignal(new ShowWinnerSignal(GetWinner(message.ScoreStates)));
            }
        }
        
        private int GetWinner(List<ScoreState> scoreStates) {
            var winner = 0;
            var maxScore = int.MinValue;
            foreach (var scoreState in scoreStates) {
                if (scoreState.Score > maxScore) {
                    maxScore = scoreState.Score;
                    winner = scoreState.PlayerId;
                }
            }

            return winner;
        }

        public void Initialize() {
        }

        public void Dispose() {
            SignalBus.UnSubscribeFromAll(this);
        }
    }
}