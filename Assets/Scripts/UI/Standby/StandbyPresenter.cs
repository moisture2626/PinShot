using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Scenes.MainGame.UI;

namespace PinShot.UI {

    /// <summary>
    /// ゲーム開始前カウントダウン画面のPresenter
    /// </summary>
    public class StandbyPresenter {
        private StandbyWindow _view;

        public void Initialize(StandbyWindow view) {
            _view = view;
            CountDown(_view.destroyCancellationToken).Forget();
        }

        /// <summary>
        /// カウントダウン後、ウィンドウを閉じる
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask CountDown(CancellationToken token) {
            for (int i = 3; i > 0; i--) {
                _view.SetText(i.ToString());
                await UniTask.Delay(1000, cancellationToken: token);
            }
            _view.SetText("GO!");
            await UniTask.Delay(1000, cancellationToken: token);

            _view.Close();
        }
    }
}
