using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using PinShot.Singletons;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    public class DescriptionWindow : BaseWindow {
        [SerializeField] private Button _closeButton;
        public Observable<Unit> CloseButton => _closeButton.OnClickAsObservable();

        public static async UniTask OpenAsync(CancellationToken token) {
            var window = WindowManager.Open<DescriptionWindow>();
            var presenter = new DescriptionPresenter();
            presenter.Initialize(window);
            await window.OnDestroyAsync().AttachExternalCancellation(token);
        }
    }
}
