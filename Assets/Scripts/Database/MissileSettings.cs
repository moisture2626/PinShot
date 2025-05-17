using UnityEngine;

namespace PinShot.Database {
    /// <summary>
    /// ミサイルの設定を管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "MissileSettings", menuName = "Scriptable Objects/MissileSettings")]
    public class MissileSettings : BaseSingleTable<MissileSettings> {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _explosionLifeTime = 0.3f;
        [SerializeField] private float _explosionEffectLifetime = 1f;
        [SerializeField] private float _explosionForce = 5f;
        public float Speed => _speed;
        public float Damage => _damage;
        public float ExplosionLifeTime => _explosionLifeTime;
        public float ExplosionEffectLifetime => _explosionEffectLifetime;
        public float ExplosionForce => _explosionForce;
    }


}
