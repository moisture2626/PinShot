using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "BallLauncherSettings", menuName = "Scriptable Objects/BallLauncherSettings")]
    public class BallLauncherSettings : BaseSingleTable<BallLauncherSettings> {
        [SerializeField] private float _launchForce = 10f;
        [SerializeField] private float _launchInterval = 1f;

        public float LaunchForce => _launchForce;
        public float LaunchInterval => _launchInterval;

    }
}
