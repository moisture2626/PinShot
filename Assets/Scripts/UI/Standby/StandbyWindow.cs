using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using PinShot.UI;
using TMPro;
using UnityEngine;

namespace PinShot.Scenes.MainGame.UI {
    /// <summary>
    /// ゲーム開始のカウントダウン
    /// </summary>
    public class StandbyWindow : BaseWindow {
        [SerializeField] private TMP_Text _countdownText;

        /// <summary>
        /// 開いてから閉じるまで待機
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async UniTask OpenAsync(CancellationToken token) {
            var window = WindowManager.Open<StandbyWindow>();
            var presenter = new StandbyPresenter();
            presenter.Initialize(window);

            await window.OnDestroyAsync().AttachExternalCancellation(token);
        }

        public void SetText(string text) {
            _countdownText.text = text;
        }
    }
}
