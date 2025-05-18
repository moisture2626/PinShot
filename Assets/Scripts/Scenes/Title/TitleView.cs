using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.Scenes.Title {
    public class TitleView : MonoBehaviour {
        [SerializeField] private Button _startButton;
        public Observable<Unit> StartButton => _startButton.OnClickAsObservable();
        [SerializeField] private Button _optionsButton;
        public Observable<Unit> OptionsButton => _optionsButton.OnClickAsObservable();
        [SerializeField] private Button _exitButton;
        public Observable<Unit> ExitButton => _exitButton.OnClickAsObservable();
        [SerializeField] private Button _descriptionButton;
        public Observable<Unit> DescriptionButton => _descriptionButton.OnClickAsObservable();
        private Canvas _canvas;

        private void Awake() {
            // Canvasを取得
            _canvas = GetComponent<Canvas>();
            if (_canvas == null) {
                return;
            }

            CameraSetup(destroyCancellationToken).Forget();
        }

        private async UniTask CameraSetup(CancellationToken token) {
            await UniTask.WaitUntil(() => Camera.main != null, cancellationToken: token);
            _canvas.worldCamera = Camera.main;
        }
    }
}
