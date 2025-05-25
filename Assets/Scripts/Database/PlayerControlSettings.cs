using UnityEngine;
using UnityEngine.InputSystem;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "PlayerControllSettings", menuName = "Scriptable Objects/PlayerControlSettings")]
    public class PlayerControlSettings : BaseSingleTable<PlayerControlSettings> {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _fireInterval = 1f;
        [SerializeField] private float _minInterval = 0.2f;

        [SerializeField] private InputAction _moveAction;
        [SerializeField] private InputAction _shotAction;

        public float MoveSpeed => _moveSpeed;
        public float FireInterval => _fireInterval;
        public float MinInterval => _minInterval;
        public InputAction MoveAction => _moveAction;
        public InputAction ShotAction => _shotAction;
    }
}
