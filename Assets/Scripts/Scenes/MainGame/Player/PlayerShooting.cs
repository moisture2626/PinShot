using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    /// <summary>
    /// 射撃の制御
    /// </summary>
    public class PlayerShooting {
        private PlayerControlSettings _playerControlSettings;
        private MissileLauncher _missileLauncher;
        private float _fireInterval;
        private float _maxInterval;
        private float _minInterval;
        private float _lastFireTime = 0;

        public void Initialize(PlayerControlSettings playerControlSettings, MissileLauncher launcher) {
            _playerControlSettings = playerControlSettings;
            _missileLauncher = launcher;
            _fireInterval = playerControlSettings.FireInterval;
            _maxInterval = playerControlSettings.FireInterval;
            _minInterval = playerControlSettings.MinInterval;
        }

        public void AddFireInterval(float add) {
            add = -add;
            _fireInterval += add;
            if (_fireInterval < _minInterval) {
                _fireInterval = _minInterval;
            }
        }

        public void ResetFireInterval() {
            _fireInterval = _maxInterval;
        }

        public void Fire(int addPower) {
            if (Time.time - _lastFireTime >= _fireInterval) {
                _missileLauncher.Fire(addPower);
                _lastFireTime = Time.time;
            }
        }
    }
}
