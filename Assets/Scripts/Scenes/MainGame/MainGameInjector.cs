using PinShot.Event;
using PinShot.Scenes.MainGame.Item;
using PinShot.Scenes.MainGame.Player;
using PinShot.Scenes.MainGame.Stage;
using PinShot.UI;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// メインゲームシーンのDI
    /// </summary>
    public class MainGameInjector : LifetimeScope {
        [SerializeField] private StageInjector _stageInjector;
        [SerializeField] private ItemInjector _itemInjector;
        [SerializeField] private PlayerInjector _playerInjector;
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private Canvas _uiCanvas;
        private CompositeDisposable _disposables;

        protected override void Configure(IContainerBuilder builder) {
            VContainerSettings.Instance.EnableDiagnostics = true;
            _uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiCanvas.worldCamera = Camera.main;

            // イベント系
            _disposables = new(){
                new EventManager<GameStateEvent>(),
                new EventManager<ScoreEvent>(),
                new EventManager<ItemDropEvent>(),
            };

            // コンテナに登録
            builder.Register<ScoreModel>(Lifetime.Scoped);
            builder.RegisterComponent(_gameUI).AsImplementedInterfaces();

            _itemInjector.Configure(builder);
            _stageInjector.Configure(builder);
            _playerInjector.Configure(builder);

            builder.RegisterEntryPoint<MainGamePresenter>(Lifetime.Scoped);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            _disposables?.Dispose();
        }
    }
}
