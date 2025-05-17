using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "BallManagerSettings", menuName = "Scriptable Objects/BallManagerSettings")]
    public class BallManagerSettings : BaseSingleTable<BallManagerSettings> {
        [SerializeField] private float _launchForce = 10f;
        [SerializeField] private float _launchInterval = 1f;

        public float LaunchForce => _launchForce;
        public float LaunchInterval => _launchInterval;

    }
}
