using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "PlayerControllSettings", menuName = "Scriptable Objects/PlayerControlSettings")]
    public class PlayerControlSettings : BaseSingleTable<PlayerControlSettings> {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _shootInterval = 1f;

        public float MoveSpeed => _moveSpeed;
        public float ShootInterval => _shootInterval;
    }
}
