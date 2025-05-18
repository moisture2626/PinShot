using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Const;
using PinShot.Event;
using PinShot.Singletons;
using R3;

namespace PinShot.UI {
    /// <summary>
    /// リザルト画面のPresenter
    /// </summary>
    public class ResultPresenter {
        private ResultWindow _view;

        public void Initialize(ResultWindow view) {
            _view = view;
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
            int score = 100; // ここは実際のスコアを取得する処理に置き換える

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
            SceneChanger.Instance.ChangeScene(ConstScene.Title).Forget();
            _view.Close();
        }
    }
}
