using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using PinShot.Singletons;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    public class LicenseWindow : BaseWindow {
        [SerializeField] private Button _closeButton;
        public Observable<Unit> CloseButton => _closeButton.OnClickAsObservable();

        public static async UniTask OpenAsync(CancellationToken token) {
            var window = WindowManager.Open<LicenseWindow>();
            var presenter = new LicensePresenter();
            presenter.Initialize(window);
            await window.OnDestroyAsync().AttachExternalCancellation(token);
        }
    }
}
