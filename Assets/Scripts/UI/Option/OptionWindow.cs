using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using PinShot.Singletons;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.UI {
    public class OptionWindow : BaseWindow {
        [SerializeField] private Slider _bgmVolumeSlider;
        public Observable<float> BgmVolumeSliderAsObservable => _bgmVolumeSlider.OnValueChangedAsObservable();
        [SerializeField] private Slider _seVolumeSlider;
        public Observable<float> SeVolumeSliderAsObservable => _seVolumeSlider.OnValueChangedAsObservable();
        [SerializeField] private Button _closeButton;
        public Observable<Unit> CloseButtonAsObservable => _closeButton.OnClickAsObservable();
        [SerializeField] private Button _licenseButton;
        public Observable<Unit> LicenseButtonAsObservable => _licenseButton.OnClickAsObservable();

        public static async UniTask OpenAsync(CancellationToken token) {
            var window = WindowManager.Open<OptionWindow>();
            var presenter = new OptionPresenter();
            await presenter.Initialize(window, token);
            await window.OnDestroyAsync().AttachExternalCancellation(token);
        }

        public void SliderInitialize(float bgmVolume, float seVolume) {
            _bgmVolumeSlider.value = bgmVolume;
            _seVolumeSlider.value = seVolume;
        }
    }
}
