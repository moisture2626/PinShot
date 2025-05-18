using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    public class BallLaunchTrigger : MonoBehaviour, IBallEnter {
        private float _launchVelocity;
        private float _randomRange;
        public void Initialize(BallManagerSettings settings) {
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
