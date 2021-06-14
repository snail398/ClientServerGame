namespace Shared {
    public class CoinState {
        public int CoinId;
        public bool IsActive;
        public Vector3d Position;

        public CoinState(int coinId) {
            CoinId = coinId;
        }

        public CoinState() {
        }
    }
}