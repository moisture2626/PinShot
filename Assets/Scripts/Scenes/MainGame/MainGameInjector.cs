using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using PinShot.Scenes.MainGame.Ball;
using PinShot.Scenes.MainGame.Item;
using PinShot.Scenes.MainGame.Player;
using PinShot.Singletons;
using PinShot.UI;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// メインゲームシーンのDI
    /// </summary>
    public class MainGameInjector : MonoBehaviour {
        [SerializeField] private PlayerInjector _playerInjector;
        [SerializeField] private BallManagerInjector _ballManagerInjector;
        [SerializeField] private ItemManagerInjector _itemManagerInjector;
        [SerializeField] private MainGameManager _mainGameManager;
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private GameUI _gameUI;

        private void Awake() {
            Inject(destroyCancellationToken).Forget();
        }

        private async UniTask Inject(CancellationToken token) {
            _uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiCanvas.worldCamera = Camera.main;
            // まずSaveDataManagerを待機
            await UniTask.WaitUntil(() => SaveDataManager.Instance, cancellationToken: token);
            // その後、Singletonを待機
            await UniTask.WhenAll(
                UniTask.WaitUntil(() => MasterDataManager.Instance, cancellationToken: token),
                UniTask.WaitUntil(() => WindowManager.Instance, cancellationToken: token),
                UniTask.WaitUntil(() => SoundManager.Instance, cancellationToken: token)
            );

            // イベント系
            _ = new EventManager<GameStateEvent>();
            _ = new EventManager<ScoreEvent>();

            // ゲーム内オブジェクトの初期化
            _playerInjector.Initialize();
            _ballManagerInjector.Initialize(_gameUI);
            _itemManagerInjector.Initialize();
            _mainGameManager.Initialize(_gameUI);

            // 初期化終了後、ゲーム開始
            _mainGameManager.BeginGame();
        }

        void OnDestroy() {
            EventManager<GameStateEvent>.DisposeAll();
            EventManager<ScoreEvent>.DisposeAll();
        }
    }
}
