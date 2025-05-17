using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    [CreateAssetMenu(fileName = "PlayerControllSettings", menuName = "Scriptable Objects/PlayerControlSettings")]
    public class PlayerControlSettings : ScriptableObject {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _shootInterval = 1f;

        public float MoveSpeed => _moveSpeed;
        public float ShootInterval => _shootInterval;
    }
}
