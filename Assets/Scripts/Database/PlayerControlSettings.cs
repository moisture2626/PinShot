using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "PlayerControllSettings", menuName = "Scriptable Objects/PlayerControlSettings")]
    public class PlayerControlSettings : BaseSingleTable<PlayerControlSettings> {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _fireInterval = 1f;

        public float MoveSpeed => _moveSpeed;
        public float FireInterval => _fireInterval;
    }
}
