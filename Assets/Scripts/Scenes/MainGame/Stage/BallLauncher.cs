using PinShot.Database;
using PinShot.Scenes.MainGame.Ball;
using PinShot.Singletons;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace PinShot.Scenes.MainGame.Stage {

    /// <summary>
    /// 発射台（実質ただのプール）
    /// </summary>
    public class BallLauncher : MonoBehaviour {
        [SerializeField] private BallObject _ballPrefab;
        private ObjectPool<BallObject> _ballPool;
        private BallSettings _ballSettings;

        [Inject]
        public void Construct(MasterDataManager mst) {
            _ballSettings = mst.GetTable<BallSettings>();
            _ballPool = new ObjectPool<BallObject>(
                CreateBall,
                OnGetBall,
                OnReleaseBall,
                OnDestroyBall,
                defaultCapacity: 20
            );
        }

        public BallObject GetBall() {
            return _ballPool.Get();
        }
        public void ReleaseBall(BallObject ball) {
            if (ball == null)
                return;

            _ballPool.Release(ball);
        }
        public void Clear() {
            // プール内のボールを全て破棄
            _ballPool.Clear();
        }

        #region Pool
        private BallObject CreateBall() {
            var ball = Instantiate(_ballPrefab, transform);
            return ball;
        }
        private void OnGetBall(BallObject ball) {
            ball.gameObject.SetActive(true);
            ball.transform.position = transform.position;
            ball.Initialize(_ballSettings);
        }
        private void OnReleaseBall(BallObject ball) {
            ball.gameObject.SetActive(false);
        }
        private void OnDestroyBall(BallObject ball) {
            Destroy(ball.gameObject);
        }
        #endregion
    }
}
