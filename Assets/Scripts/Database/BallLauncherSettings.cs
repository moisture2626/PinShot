using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "BallLauncherSettings", menuName = "Scriptable Objects/BallLauncherSettings")]
    public class BallLauncherSettings : BaseSingleTable<BallLauncherSettings> {
        [SerializeField] private float _launchForce = 10f;
        [SerializeField] private float _forceRandomRange = 2f;
        [SerializeField] private float _launchIntervalMax = 30f;
        [SerializeField] private float _launchIntervalMin = 1f;

        // インターバルが最短になるまでに発射する数
        [SerializeField] private int _intervalMaxCount = 50;

        // この数ぶん落としたらゲームオーバー
        [SerializeField] private int _gameOverCount = 3;

        public float LaunchForce => _launchForce;
        public float ForceRandomRange => _forceRandomRange;
        public float LaunchIntervalMax => _launchIntervalMax;
        public float LaunchIntervalMin => _launchIntervalMin;
        public int IntervalMaxCount => _intervalMaxCount;
        public int GameOverCount => _gameOverCount;

    }
}
