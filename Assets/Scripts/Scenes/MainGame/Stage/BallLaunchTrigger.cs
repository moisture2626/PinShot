using PinShot.Database;
using PinShot.Singletons;
using UnityEngine;
using VContainer;

namespace PinShot.Scenes.MainGame.Ball {
    public class BallLaunchTrigger : MonoBehaviour, IBallEnter {
        private float _launchVelocity;
        private float _randomRange;
        [Inject]
        public void Construct(MasterDataManager mst) {
            var settings = mst.GetTable<BallLauncherSettings>();
            _launchVelocity = settings.LaunchForce;
            _randomRange = settings.ForceRandomRange;
        }


        public (float offset, float velocity) OnEnterBall(Vector2 hitPosition) {
            return (Random.Range(-_randomRange, _randomRange), _launchVelocity);
        }

        public void OnExitBall(Vector2 hitPosition) {
            // Do nothing
        }

    }
}
