using System;
using UnityEngine;

namespace Shared.Coins {
    public class CoinView : MonoBehaviour, ITakeable {
        public event Action<int, int> OnTake;

        private int _CoinId;

        public void Init(int coinId) {
            _CoinId = coinId;
        }
        
        public void Take(int id) {
            OnTake?.Invoke(_CoinId, id);
        }
    }

    public interface ITakeable {
        void Take(int id);
    }
}