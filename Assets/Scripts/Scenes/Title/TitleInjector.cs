using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.Title {
    /// <summary>
    /// タイトルシーンのDI
    /// </summary>
    public class TitleInjector : LifetimeScope {
        [SerializeField] private TitleView _view;

        protected override void Configure(IContainerBuilder builder) {
            // Viewを登録
            builder.RegisterComponent(_view);
            // Presenterを登録
            builder.RegisterEntryPoint<TitlePresenter>(Lifetime.Scoped);
        }

    }
}
