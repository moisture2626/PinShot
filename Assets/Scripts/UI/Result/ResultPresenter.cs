using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Const;
using PinShot.Event;
using PinShot.Scenes.MainGame;
using PinShot.Singletons;
using R3;
using UnityEngine;

namespace PinShot.UI {
    /// <summary>
    /// リザルト画面のPresenter
    /// </summary>
    public class ResultPresenter {
        private ResultWindow _view;
        private ScoreManager _scoreManager;

        public void Initialize(ResultWindow view, ScoreManager scoreManager) {
            _view = view;
            _scoreManager = scoreManager;
            Subscribe();
            // 結果表示
            ShowResult(_view.destroyCancellationToken).Forget();
        }

        /// <summary>
        /// ウインドウのアクションを購読
        /// </summary>
        private void Subscribe() {
            _view.OnRetryButtonClicked
                .Subscribe(_ => Retry())
                .AddTo(_view.destroyCancellationToken);

            _view.OnTitleButtonClicked
                .Subscribe(_ => Title())
                .AddTo(_view.destroyCancellationToken);
        }

        /// <summary>
        /// スコア表示演出
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask ShowResult(CancellationToken token) {
            int score = _scoreManager.Score;

            await _view.ShowScore(score, token);
            await UniTask.Delay(500, cancellationToken: token);
            _view.ShowButtons();
        }

        /// <summary>
        /// リトライ
        /// </summary>
        private void Retry() {
            EventManager<GameStateEvent>.TriggerEvent(GameStateEvent.Create(GameState.Standby));
            _view.Close();
        }

        /// <summary>
        /// タイトルに戻る
        /// </summary>
        private void Title() {
            ReturnToTitleFlow(_view.destroyCancellationToken).Forget();
        }

        private async UniTask ReturnToTitleFlow(CancellationToken token) {
            await ScreenFade.Instance.FadeInAsync(0.2f, Color.black, token);
            SceneChanger.Instance.ChangeScene(ConstScene.Title).Forget();
            _view.Close();
        }
    }
}
