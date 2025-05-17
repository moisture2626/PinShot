using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    public class BallLaunchTrigger : MonoBehaviour, IBallEnter {
        private float _launchVelocity;
        public void Initialize(BallLauncherSettings settings) {
            _launchVelocity = settings.LaunchForce;

        }
        /// <summary>
        /// 隕石にぶつかったときの処理
        /// </summary>
        /// <param name="hitPosition">ぶつかった位置</param>
        /// <returns></returns>
        public Vector2 OnStayBall(Vector2 hitPosition) {
            return Vector2.up * _launchVelocity;
        }

        public void OnExitBall(Vector2 hitPosition) {
            // Do nothing
        }

    }
}
