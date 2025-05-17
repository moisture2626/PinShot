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
        private float _lastFireTime = 0;

        public void Initialize(PlayerControlSettings playerControlSettings, MissileLauncher launcher) {
            _playerControlSettings = playerControlSettings;
            _missileLauncher = launcher;
            _fireInterval = playerControlSettings.FireInterval;
        }

        public void Fire() {
            if (Time.time - _lastFireTime >= _fireInterval) {
                _missileLauncher.Fire();
                _lastFireTime = Time.time;
            }
        }
    }
}
