using System;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI {
    public class ScoreItem : MonoBehaviour {
        [SerializeField]
        private Text _Text;

        private int _PlayerId;
        
        public void Init(int playerId, int initialScore) {
            _PlayerId = playerId;
            _Text.text = $"Player: {playerId} Score: {initialScore}";
        }

        public void SetScore(int score) {
            _Text.text = $"Player: {_PlayerId} Score: {score}";
        }

        private void OnValidate() {
            _Text = GetComponentInChildren<Text>();
        }
    }
}