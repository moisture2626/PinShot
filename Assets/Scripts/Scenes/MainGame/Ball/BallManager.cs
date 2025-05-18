using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using PinShot.Event;
using R3;
using UnityEngine;
using UnityEngine.Pool;

namespace PinShot.Scenes.MainGame.Ball {
    /// <summary>
    /// ボールを発射と管理をするクラス
    /// </summary>
    public class BallManager : MonoBehaviour {
        [SerializeField] private BallObject _ballPrefab;
        [SerializeField] private BallLaunchTrigger _trigger;
        [SerializeField] private Transform _deadLine;
        private ObjectPool<BallObject> _ballPool;
        private BallSettings _ballSettings;
        private BallManagerSettings _launcherSettings;
        private CancellationDisposable _timerCancellation;

        Subject<float> _timerSubject = new();
        public Observable<float> Timer => _timerSubject;

        private bool _isRunning = false;
        private int _launchCount = 0;

        private Health _health;

        public void Initialize(BallManagerSettings launcherSettings, BallSettings ballSettings) {
            _ballSettings = ballSettings;
            _launcherSettings = launcherSettings;
            _trigger.Initialize(launcherSettings);
            _ballPool = new ObjectPool<BallObject>(
                CreateBall,
                OnGetBall,
                OnReleaseBall,
                OnDestroyBall
            );

            // ゲームオーバーの処理をHealthでやる
            _health = new Health();
            _health.Current
                .Where(_ => _isRunning)
                .Subscribe(h => {
                    Debug.Log($"Miss! Remain: {h}");
                    if (h <= 0) {
                        // ゲームオーバー
                        EventManager<GameStateEvent>.TriggerEvent(new GameStateEvent(GameState.GameOver));
                    }
                });

            // イベントの購読
            EventManager<GameStateEvent>.Subscribe(this, ev => {
                if (ev.State == GameState.Standby) {
                    Debug.Log("Standby");
                    _health.Initialize(launcherSettings.GameOverCount);
                }
                if (ev.State == GameState.Play && !_isRunning) {
                    Debug.Log("Play");
                    BeginLaunch();
                }
                if (ev.State == GameState.GameOver) {
                    Debug.Log("GameOver");
                    Stop();
                }
            });


        }

        /// <summary>
        /// 発射を開始する
        /// </summary>
        public void BeginLaunch() {
            _launchCount = 0;
            _isRunning = true;
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

            // はじめに一発
            LaunchBall(token).Forget();

            // インターバルを計算
            var interval = GetInterval();

            while (!token.IsCancellationRequested) {
                elapsedTime += Time.deltaTime;

                var remain = interval - elapsedTime;

                _timerSubject.OnNext(remain);
                if (remain <= 0) {
                    LaunchBall(token).Forget();
                    // 次のインターバルを計算
                    interval = GetInterval();
                    elapsedTime = 0;
                }
                await UniTask.Yield(token);
            }
        }

        /// <summary>
        /// 今のインターバル時間を計算
        /// </summary>
        /// <returns></returns>
        private float GetInterval() {
            return Mathf.Lerp(
                _launcherSettings.LaunchIntervalMax,
                _launcherSettings.LaunchIntervalMin,
                (float)_launchCount / _launcherSettings.IntervalMaxCount
            );
        }

        /// <summary>
        /// 1発だけ発射
        /// </summary>
        private async UniTask LaunchBall(CancellationToken token) {
            var ball = _ballPool.Get();
            _launchCount++;
            // 耐久力がなくなるか、デッドラインを越えるまで待機
            await UniTask.WhenAny(
                WaitForOverDeadLine(ball, token),
                UniTask.WaitWhile(() => ball.Health.Current.Value > 0, cancellationToken: token)
            );
            // プールに戻す
            _ballPool.Release(ball);

            // アクティブなボールが無くなったら再度発射
            if (_ballPool.CountActive <= 0) {
                _health.TakeDamage(1);
                if (_health.Current.Value > 0) {
                    // 再度発射
                    BeginLaunch();
                }
            }
        }

        /// <summary>
        /// ボールがデッドラインを越えるまで待機
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask WaitForOverDeadLine(BallObject ball, CancellationToken token) {
            await UniTask.WaitUntil(() => ball.transform.position.y < _deadLine.position.y, cancellationToken: token);
        }

        private void Stop() {
            _isRunning = false;
            _timerCancellation?.Dispose();
            _timerCancellation = null;
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
