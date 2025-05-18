using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Singletons;
using R3;
using UnityEngine;

namespace PinShot.UI {
    public class OptionPresenter {
        private OptionWindow _view;
        public async UniTask Initialize(OptionWindow view, CancellationToken token) {
            _view = view;
            await UniTask.WaitUntil(() => SoundManager.Instance.IsLoaded, cancellationToken: token);

            var bgmVolume = SoundManager.Instance.BGMVolume;
            var seVolume = SoundManager.Instance.SEVolume;
            _view.SliderInitialize(bgmVolume, seVolume);
            Subscribe();
        }

        private void Subscribe() {
            Debug.Log("OptionPresenter Subscribe");
            _view.BgmVolumeSliderAsObservable
                .Subscribe(value => {
                    SoundManager.Instance.SetBGMMasterVolume(value);
                })
                .AddTo(_view);
            _view.SeVolumeSliderAsObservable
                .Subscribe(value => {
                    SoundManager.Instance.SetSEMasterVolume(value);
                })
                .AddTo(_view);

            _view.CloseButtonAsObservable
                .Subscribe(_ => {
                    SoundManager.Instance.SaveVolumeSettings();
                    _view.Close();
                    Debug.Log("Close");
                })
                .AddTo(_view);

            _view.LicenseButtonAsObservable
                .Subscribe(_ => {
                    // ライセンス画面
                    LicenseWindow.OpenAsync(_view.GetCancellationTokenOnDestroy()).Forget();
                })
                .AddTo(_view);
        }
    }
}
