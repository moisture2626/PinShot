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
        /// <summary>
        /// 隕石にぶつかったときの処理
        /// </summary>
        /// <param name="hitPosition">ぶつかった位置</param>
        /// <returns></returns>
        public Vector2 OnStayBall(Vector2 hitPosition) {
            return Vector2.up * _launchVelocity;
        }

        public float OnEnterBall(Vector2 hitPosition) {
            return Random.Range(-_randomRange, _randomRange);
        }

        public void OnExitBall(Vector2 hitPosition) {
            // Do nothing
        }

    }
}
