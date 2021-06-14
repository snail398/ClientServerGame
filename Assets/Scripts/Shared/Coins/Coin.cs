namespace Shared.Coins {
    public class Coin {
        private CoinView _CoinView;
        private int _Id; 
        private bool _IsActive;

        public Coin(CoinView coinView, int id) {
            _CoinView = coinView;
            _Id = id;
            _IsActive = true;
        }

        public int Id => _Id;

        public bool IsActive {
            get { return _IsActive; }
            set  {
                _CoinView.gameObject.SetActive(value);
                _IsActive = value;
            }
        }

        public Vector3d Position => new Vector3d {
            X = _CoinView.transform.position.x,
            Y = _CoinView.transform.position.y,
            Z = _CoinView.transform.position.z
        };
    }
}