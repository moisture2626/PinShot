using Cysharp.Threading.Tasks;
using PinShot.Const;
using PinShot.Singletons;
using PinShot.UI;
using R3;
using VContainer.Unity;

#if !UNITY_EDITOR

using UnityEngine;

#endif
namespace PinShot.Scenes.Title {
    public class TitlePresenter : IInitializable {
        private TitleView _view;

        public TitlePresenter(TitleView view) {
            _view = view;
        }

        public void Initialize() {
            Subscribe();
            SoundManager.Instance.PlayBGM("Title");
        }
        private void Subscribe() {
            _view.StartButton
                .Subscribe(_ => {
                    SceneChanger.Instance.ChangeScene(ConstScene.MainGame).Forget();
                })
                .AddTo(_view);

            _view.OptionsButton
                .Subscribe(_ => {
                    OptionWindow.OpenAsync(_view.destroyCancellationToken).Forget();
                })
                .AddTo(_view);

            _view.DescriptionButton
                .Subscribe(_ => {
                    // 説明画面
                    DescriptionWindow.OpenAsync(_view.destroyCancellationToken).Forget();
                })
                .AddTo(_view);

            _view.ExitButton
                .Subscribe(_ => {
#if UNITY_EDITOR
                    // エディタ上での終了
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    // ゲーム終了
                    Application.Quit();
#endif
                })
                .AddTo(_view);
        }
    }
}
