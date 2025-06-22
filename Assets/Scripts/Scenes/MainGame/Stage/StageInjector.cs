using PinShot.Scenes.MainGame.Ball;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PinShot.Scenes.MainGame.Stage {
    public class StageInjector : MonoBehaviour {
        [SerializeField] private BallLauncher _launcher;
        [SerializeField] private BallLaunchTrigger _trigger;
        [SerializeField] private Transform _deadLine;
        [SerializeField] private SlideObstacle _slideObstacle;

        public void Configure(IContainerBuilder builder) {

            builder.RegisterComponent(_launcher);
            builder.RegisterComponent(_trigger);
            builder.RegisterComponent(_slideObstacle);

            builder.RegisterEntryPoint<BallLauncherPresenter>(Lifetime.Scoped).WithParameter("deadLine", _deadLine);

        }
    }
}
