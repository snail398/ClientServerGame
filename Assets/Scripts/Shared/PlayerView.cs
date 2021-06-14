using Shared.Coins;
using UnityEngine;

namespace Shared {
    public class PlayerView : MonoBehaviour {

        private int _PlayerId;

        public void Init(int id) {
            _PlayerId = id;
        }
        
        private void OnTriggerEnter(Collider other) {
            var takeable = other.GetComponent<ITakeable>();
            takeable?.Take(_PlayerId);
        }
    }
}