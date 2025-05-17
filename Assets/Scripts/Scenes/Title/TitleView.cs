using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Const;
using PinShot.Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.Scenes.Title {
    public class TitleView : MonoBehaviour {
        [SerializeField] private Button _startButton;
        private Canvas _canvas;

        private void Awake() {
            // Canvasを取得
            _canvas = GetComponent<Canvas>();
            if (_canvas == null) {
                return;
            }

            CameraSetup(destroyCancellationToken).Forget();


            _startButton.onClick.AddListener(BeginGame);
        }

        private async UniTask CameraSetup(CancellationToken token) {
            await UniTask.WaitUntil(() => Camera.main != null, cancellationToken: token);
            _canvas.worldCamera = Camera.main;
        }

        private void BeginGame() {
            SceneChanger.Instance.ChangeScene(ConstScene.MainGame).Forget();
        }
    }
}
