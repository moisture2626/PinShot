using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Event;
using PinShot.Scenes.MainGame.Ball;
using PinShot.Scenes.MainGame.Item;
using PinShot.Singletons;
using PinShot.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// メインゲームシーンのDI
    /// </summary>
    public class MainGameInjector : LifetimeScope {
        [SerializeField] private BallManagerInjector _ballManagerInjector;
        [SerializeField] private ItemManagerInjector _itemManagerInjector;
        [SerializeField] private MainGameManager _mainGameManager;
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private GameUI _gameUI;

        protected override void Configure(IContainerBuilder builder) {
            Debug.Log("Starting registration in MainGameInjector");
            Inject(builder, destroyCancellationToken).Forget();
        }

        private async UniTask Inject(IContainerBuilder builder, CancellationToken token) {
            _uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiCanvas.worldCamera = Camera.main;
            // まずSaveDataManagerを待機
            //await UniTask.WaitUntil(() => SaveDataManager.Instance, cancellationToken: token);
            // その後、Singletonを待機
            await UniTask.WhenAll(
                //UniTask.WaitUntil(() => MasterDataManager.Instance, cancellationToken: token),
                UniTask.WaitUntil(() => WindowManager.Instance, cancellationToken: token),
                UniTask.WaitUntil(() => SoundManager.Instance, cancellationToken: token)
            );

            // イベント系
            using var stateEvent = new EventManager<GameStateEvent>();
            using var scoreEvent = new EventManager<ScoreEvent>();

            // DIコンテナに登録


            _ballManagerInjector.Initialize(_gameUI);
            _itemManagerInjector.Initialize();
            _mainGameManager.Initialize(_gameUI);

            // 初期化終了後、ゲーム開始
            await _mainGameManager.GameLoop(destroyCancellationToken);
        }

    }
}
