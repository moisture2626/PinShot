using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using PinShot.Event;
using PinShot.Scenes.MainGame.Item;
using PinShot.UI;
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
        private int _combo = 0;
        private GameUI _gameUI;

        public void Initialize(BallManagerSettings launcherSettings, BallSettings ballSettings, GameUI gameUI) {
            _ballSettings = ballSettings;
            _launcherSettings = launcherSettings;
            _trigger.Initialize(launcherSettings);
            _gameUI = gameUI;
            _ballPool = new ObjectPool<BallObject>(
                CreateBall,
                OnGetBall,
                OnReleaseBall,
                OnDestroyBall,
                defaultCapacity: 20
            );

            // ゲームオーバーの処理をHealthでやる
            _health = new Health();
            _health.Current
                .Where(_ => _isRunning)
                .Subscribe(h => {
                    _gameUI.SetLife((int)h);
                    if (h <= 0) {
                        // ゲームオーバー
                        EventManager<GameStateEvent>.TriggerEvent(new GameStateEvent(GameState.GameOver));
                    }
                });

            // イベントの購読
            EventManager<GameStateEvent>.Subscribe(this, ev => {
                if (ev.State == GameState.Standby) {
                    _gameUI.SetLife(launcherSettings.GameOverCount);
                    _health.Initialize(launcherSettings.GameOverCount);
                }
                if (ev.State == GameState.Play && !_isRunning) {
                    BeginLaunch();
                }
                if (ev.State == GameState.GameOver) {
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
                // ゲージ表示
                _gameUI.SetNextGauge(remain / interval);

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
            using var internalCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
            await UniTask.WhenAny(
                WaitForOverDeadLine(ball, internalCancellation.Token),
                WaitForBallDead(ball, internalCancellation.Token)
            );
            // プールに戻す
            _ballPool.Release(ball);

            // WhenAnyの処理を全部終わらせる
            internalCancellation.Cancel();
        }

        /// <summary>
        /// ボールがデッドラインを越えるまで待機
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask WaitForOverDeadLine(BallObject ball, CancellationToken token) {
            await UniTask.WaitUntil(() => ball.transform.position.y < _deadLine.position.y, cancellationToken: token);
            token.ThrowIfCancellationRequested();
            // ミスカウントとコンボリセット
            _combo = 0;
            _gameUI.SetCombo(_combo);
            _health.TakeDamage(1);
        }

        /// <summary>
        /// ボールの耐久力がなくなるまで待機
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask WaitForBallDead(BallObject ball, CancellationToken token) {
            await UniTask.WaitUntil(() => ball.Health.Current.Value <= 0, cancellationToken: token);
            token.ThrowIfCancellationRequested();
            // 破壊したらスコアとコンボ加算
            EventManager<ScoreEvent>.TriggerEvent(new ScoreEvent(_ballSettings.Score, _combo));
            _combo++;
            _gameUI.SetCombo(_combo);

            // アイテムドロップ
            bool drop = Random.Range(0f, 1f) < _ballSettings.ItemDropRate;
            if (drop) {
                ItemManager.DropItem(ball.transform.position);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void Stop() {
            _isRunning = false;
            _combo = 0;
            _timerCancellation?.Dispose();
            _timerCancellation = null;
            _ballPool.Clear();
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
