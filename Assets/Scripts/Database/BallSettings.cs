using UnityEngine;

namespace PinShot.Database {
    [CreateAssetMenu(fileName = "BallSettings", menuName = "Scriptable Objects/BallSettings")]
    public class BallSettings : BaseSingleTable<BallSettings> {
        [SerializeField] private float _gravityScale = 1f;
        [SerializeField] private float _mass = 1f;
        [SerializeField] private float _health = 10;
        [SerializeField] private int _score = 10;
        [SerializeField] private float _linearDumping = 0;
        [SerializeField] private float itemDropRate = 0.2f;

        public float GravityScale => _gravityScale;
        public float Health => _health;
        public float Mass => _mass;
        public float LinearDumping => _linearDumping;
        public int Score => _score;
        public float ItemDropRate => itemDropRate;
    }
}
