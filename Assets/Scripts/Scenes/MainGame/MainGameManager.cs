using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using PinShot.Scenes.MainGame.UI;
using PinShot.Singletons;
using PinShot.UI;
using R3;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    public class MainGameManager : MonoBehaviour {

        private CancellationTokenSource _gameFlowCancellation;
        private ScoreManager _scoreManager;
        private GameUI _gameUI;
        public void Initialize(GameUI gameUI) {
            _gameUI = gameUI;
            _gameFlowCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _scoreManager = new ScoreManager();
            _scoreManager.Initialize();
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public void BeginGame() {
            EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Standby));
            GameFlow(_gameFlowCancellation.Token).Forget();
        }

        /// <summary>
        /// ゲームループ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask GameFlow(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                // UIリセット
                _gameUI.Initialize();
                // スコアリセット
                _scoreManager.Reset();
                _scoreManager.OnChangeScore.Subscribe(s => {
                    // スコアUIの更新
                    _gameUI.SetScore(s);
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

                // リザルト表示
                ResultWindow.OpenAsync(_scoreManager, token).Forget();
                ScreenFade.Instance.FadeInAsync(0.2f, Color.black, token).Forget();

                // リザルトウィンドウの操作を待機
                await EventManager<GameStateEvent>.WaitForEvent(ev => ev.State == GameState.Standby, token);
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
