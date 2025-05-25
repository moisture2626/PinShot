using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using PinShot.Event;
using PinShot.Scenes.MainGame.Item;
using PinShot.Scenes.MainGame.Stage;
using PinShot.Singletons;
using PinShot.UI;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame.Ball {
    /// <summary>
    /// ボールを発射と管理をするクラス
    /// </summary>
    public class BallLauncherPresenter : IInitializable, IDisposable {
        [Inject] private BallLauncher _launcher;
        private Transform _deadLine;

        private BallSettings _ballSettings;
        private BallLauncherSettings _launcherSettings;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private CancellationDisposable _timerCancellation;

        Subject<float> _timerSubject = new();
        public Observable<float> Timer => _timerSubject;

        private bool _isRunning = false;
        private int _launchCount = 0;

        private Health _health;
        private int _combo = 0;
        [Inject] private IHealthView _healthView;
        [Inject] private IComboView _comboView;
        [Inject] private IIntervalView _nextGauge;

        public BallLauncherPresenter(MasterDataManager mst, Transform deadLine) {
            _deadLine = deadLine;
            _ballSettings = mst.GetTable<BallSettings>();
            _launcherSettings = mst.GetTable<BallLauncherSettings>();
        }

        public void Initialize() {
            Debug.Log("BallLauncherPresenter Initialized");
            // ゲームオーバーの処理をHealthでやる
            _health = new Health();
            _health.OnChangeValue
                .Where(_ => _isRunning)
                .Subscribe(h => {
                    _healthView.SetHealth(h.prev, h.current, h.max);
                    if (h.current <= 0) {
                        // ゲームオーバー
                        EventManager<GameStateEvent>.TriggerEvent(new GameStateEvent(GameState.GameOver));
                    }
                });

            // イベントの購読
            EventManager<GameStateEvent>.Subscribe(ev => {
                Debug.Log($"GameState Changed: {ev.State}");
                if (ev.State == GameState.Standby) {
                    int maxCount = _launcherSettings.GameOverCount;
                    _healthView.InitializeHealth(maxCount);
                    _health.Initialize(_launcherSettings.GameOverCount);
                }
                if (ev.State == GameState.Play && !_isRunning) {
                    BeginLaunch();
                }
                if (ev.State == GameState.GameOver) {
                    Stop();
                }
            }).AddTo(_disposables);

            _disposables.Add(_health);
        }

        /// <summary>
        /// 発射を開始する
        /// </summary>
        private void BeginLaunch() {
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
                _nextGauge.SetInterval(remain, interval);

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
            var ball = _launcher.GetBall();
            _launchCount++;
            // 耐久力がなくなるか、デッドラインを越えるまで待機
            using var internalCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
            await UniTask.WhenAny(
                WaitForOverDeadLine(ball, internalCancellation.Token),
                WaitForBallDead(ball, internalCancellation.Token)
            );
            // プールに戻す
            _launcher.ReleaseBall(ball);

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
            _comboView.SetCombo(_combo);
            _health.TakeDamage(1);
        }

        /// <summary>
        /// ボールの耐久力がなくなるまで待機
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask WaitForBallDead(BallObject ball, CancellationToken token) {
            await UniTask.WaitUntil(() => ball.Health.Current <= 0, cancellationToken: token);
            token.ThrowIfCancellationRequested();
            // 破壊したらスコアとコンボ加算
            EventManager<ScoreEvent>.TriggerEvent(new ScoreEvent(_ballSettings.Score, _combo));
            _combo++;
            _comboView.SetCombo(_combo);

            // アイテムドロップ
            bool drop = UnityEngine.Random.Range(0f, 1f) < _ballSettings.ItemDropRate;
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
            _launcher.Clear();
        }

        public void Dispose() {
            _timerCancellation?.Dispose();
            _disposables?.Dispose();
        }


    }
}
