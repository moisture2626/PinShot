using PinShot.Event;
using PinShot.Scenes.MainGame.Ball;
using PinShot.Scenes.MainGame.Item;
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
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private GameUI _gameUI;

        protected override void Configure(IContainerBuilder builder) {
            Debug.Log("Starting registration in MainGameInjector");
            _uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiCanvas.worldCamera = Camera.main;

            // イベント系
            _ = new EventManager<GameStateEvent>();
            _ = new EventManager<ScoreEvent>();

            _ballManagerInjector.Initialize(_gameUI);
            _itemManagerInjector.Initialize();
            builder.RegisterComponent(_gameUI);
            builder.Register<ScoreManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MainGamePresenter>(Lifetime.Singleton);
        }
    }
}
