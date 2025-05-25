using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame.Player {
    public class PlayerInjector : MonoBehaviour {
        [SerializeField] private PlayerView _view;
        [SerializeField] private MissileLauncher _missileLauncher;
        [SerializeField] private Transform _leftLimit;
        [SerializeField] private Transform _rightLimit;


        public void Configure(IContainerBuilder builder) {
            builder.RegisterComponent(_view);
            builder.RegisterComponent(_missileLauncher);
            builder.RegisterEntryPoint<PlayerPresenter>(Lifetime.Scoped)
                .WithParameter("leftLimit", _leftLimit)
                .WithParameter("rightLimit", _rightLimit);

        }
    }
}
