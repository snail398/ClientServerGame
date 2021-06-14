using System;
using System.Collections.Generic;
using Shared.Spawn.Signals;
using Zenject;

namespace Server.Score {
    public class ScoreServerService : IInitializable, IDisposable, IGameStateProvider<List<ScoreState>> {
        private readonly SignalBus _SignalBus;

        //Dictionary<PlayerId, Score>
        private readonly Dictionary<int, int> _Scores = new Dictionary<int, int>();
        private readonly List<ScoreState> _ScoreState = new List<ScoreState>();

        public ScoreServerService(SignalBus signalBus) {
            _SignalBus = signalBus;
        }

        public void Initialize() {
            _SignalBus.Subscribe<PlayerSpawnedServerSignal>(OnPlayerSpawn, this);
            _SignalBus.Subscribe<TakeCoinSignal>(OnTakeCoin, this);
        }

        public bool HasWinner() {
            //Dictionary<Score, List<PlayerId>>
            Dictionary<int, List<int>> scorePlayers = new Dictionary<int, List<int>>();
            var maxScore = int.MinValue;
            foreach (var scoreKvp in _Scores) {
                if (scoreKvp.Value > maxScore)
                    maxScore = scoreKvp.Value;
                if (!scorePlayers.TryGetValue(scoreKvp.Value, out var list)) {
                    list = new List<int>();
                    scorePlayers[scoreKvp.Value] = list;
                }
                scorePlayers[scoreKvp.Value].Add(scoreKvp.Key);
            }

            var maxLevelPlayers = scorePlayers[maxScore];
            return maxLevelPlayers.Count <= 1;
        }
        
        private void OnTakeCoin(TakeCoinSignal obj) {
            _Scores.TryGetValue(obj.PlayerId, out var previousScore);
            _Scores[obj.PlayerId] = previousScore + 1;
        }

        private void OnPlayerSpawn(PlayerSpawnedServerSignal obj) {
            _Scores.Add(obj.PlayerId, 0);
            _ScoreState.Add(new ScoreState(obj.PlayerId));
        }

        public void Dispose() {
            _SignalBus.UnSubscribeFromAll(this);
        }

        public List<ScoreState> State {
            get {
                for (int i = 0; i < _ScoreState.Count; i++) {
                    var score = _Scores[i];
                    _ScoreState[i].Score = score;
                }
                return _ScoreState;
            }
        }
    }
}