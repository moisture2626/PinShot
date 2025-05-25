using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using PinShot.Extensions;
using PinShot.Scenes.MainGame.UI;
using PinShot.Singletons;
using PinShot.UI;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame {
    public class MainGamePresenter : IStartable, IDisposable {

        private CancellationDisposable _gameFlowCancellation;
        private ScoreModel _score;
        private IGameUIInitializer _uiInitializer;
        private IScoreView _scoreView;
        private IHighScoreView _highScoreView;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public MainGamePresenter(IGameUIInitializer uIInitializer, IHighScoreView highScoreView, IScoreView scoreView, ScoreModel scoreManager) {
            _uiInitializer = uIInitializer;
            _highScoreView = highScoreView;
            _scoreView = scoreView;
            _gameFlowCancellation = new();
            _score = scoreManager;
        }

        public void Start() {
            EventManager<GameStateEvent>
                .Subscribe(ev => Debug.Log($"GameState Changed: {ev.State}".SetColor(Color.green)))
                .AddTo(_disposables);

            _gameFlowCancellation = new CancellationDisposable();
            GameLoop(_gameFlowCancellation.Token).Forget();
        }


        /// <summary>
        /// ゲームループ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask GameLoop(CancellationToken token) {
            Debug.Log("GameLoop Start");

            // BGM再生開始
            SoundManager.Instance.PlayBGM("Game", 0.2f);
            EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Standby));

            while (!token.IsCancellationRequested) {
                // UIリセット
                _uiInitializer.Initialize();
                _highScoreView.SetHighScore(_score.HighScore);
                // スコアリセット
                _score.Reset();
                _score.OnChangeScore.Subscribe(s => {
                    // スコアUIの更新
                    _scoreView.SetScore(s);
                }).AddTo(token);

                // 開始前のカウントダウンを待機
                await StandbyWindow.OpenAsync(token);
                token.ThrowIfCancellationRequested();
                // ゲーム開始
                EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Play));

                // ゲームオーバーまで待機
                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.GameOver, token);
                token.ThrowIfCancellationRequested();

                // ゲームオーバー演出
                // 画面暗転
                Time.timeScale = 0.5f;
                await ScreenFade.Instance.FadeOutAsync(0.5f, Color.black, token);
                Time.timeScale = 1;
                EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Result));
                // スコアを保存
                _score.Save();
                // リザルト表示
                ResultWindow.OpenAsync(_score, token).Forget();
                ScreenFade.Instance.FadeInAsync(0.2f, Color.black, token).Forget();

                // リザルトウィンドウの操作を待機
                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Standby, token);
                token.ThrowIfCancellationRequested();
            }
        }

        public void Dispose() {
            _gameFlowCancellation?.Dispose();
            _disposables.Dispose();
        }
    }
}
