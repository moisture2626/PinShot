using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using R3;
using UnityEngine;
using UnityEngine.Pool;

namespace PinShot.Scenes.MainGame.Ball {
    /// <summary>
    /// ボールを発射と管理をするクラス
    /// </summary>
    public class BallLauncher : MonoBehaviour {
        [SerializeField] private BallObject _ballPrefab;
        [SerializeField] private BallLaunchTrigger _trigger;
        private ObjectPool<BallObject> _ballPool;
        private List<BallObject> _activeBalls = new();
        private BallSettings _ballSettings;
        private BallLauncherSettings _launcherSettings;
        private CancellationDisposable _timerCancellation;

        Subject<float> _timerSubject = new();
        public Observable<float> Timer => _timerSubject;

        public void Initialize(BallLauncherSettings launcherSettings, BallSettings ballSettings) {
            _ballSettings = ballSettings;
            _launcherSettings = launcherSettings;
            _trigger.Initialize(launcherSettings);
            _ballPool = new ObjectPool<BallObject>(
                CreateBall,
                OnGetBall,
                OnReleaseBall,
                OnDestroyBall
            );

            BeginLaunch();
        }

        /// <summary>
        /// 発射を開始する
        /// </summary>
        public void BeginLaunch() {
            _timerCancellation?.Dispose();
            _timerCancellation = new CancellationDisposable();
            IntervalLaunch(_timerCancellation.Token).Forget();
        }

        /// <summary>
        /// 一定時間ごとに発射する
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask IntervalLaunch(CancellationToken token) {
            float elapsedTime = 0;
            while (!token.IsCancellationRequested) {
                elapsedTime += Time.deltaTime;
                var remain = _launcherSettings.LaunchInterval - elapsedTime;
                _timerSubject.OnNext(remain);
                if (remain <= 0) {
                    LaunchBall(token).Forget();
                    elapsedTime = 0;
                }
                await UniTask.Yield(token);
            }
        }

        /// <summary>
        /// 1発だけ発射
        /// </summary>
        private async UniTask LaunchBall(CancellationToken token) {
            var ball = _ballPool.Get();

            // 耐久力がなくなったらプールに戻す
            await UniTask.WaitWhile(() => ball.Health.Current.Value > 0, cancellationToken: token);
            _ballPool.Release(ball);
        }

        void OnDestroy() {
            _timerCancellation?.Dispose();
        }

        #region Pool
        private BallObject CreateBall() {
            var ball = Instantiate(_ballPrefab, transform);
            return ball;
        }
        private void OnGetBall(BallObject ball) {
            ball.gameObject.SetActive(true);
            _activeBalls.Add(ball);
            ball.transform.position = transform.position;
            ball.Initialize(_ballSettings);
        }
        private void OnReleaseBall(BallObject ball) {
            ball.gameObject.SetActive(false);
            _activeBalls.Remove(ball);
        }
        private void OnDestroyBall(BallObject ball) {
            Destroy(ball.gameObject);
        }
        #endregion
    }
}
