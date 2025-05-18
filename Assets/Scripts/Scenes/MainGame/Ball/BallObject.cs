using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    public class BallObject : MonoBehaviour {
        private Collider2D _collider;
        public Collider2D Collider2D {
            get {
                if (_collider == null) {
                    _collider = GetComponent<Collider2D>();
                }
                return _collider;
            }
        }
        private Rigidbody2D _rigidbody;
        public Rigidbody2D Rigidbody2D {
            get {
                if (_rigidbody == null) {
                    _rigidbody = GetComponent<Rigidbody2D>();
                }
                return _rigidbody;
            }
        }

        private Health _health;
        public Health Health => _health;

        private BallSettings _settings;

        public void Initialize(BallSettings settings) {
            _settings = settings;
            _health ??= new Health();
            _health.Initialize(_settings.Health);
            Rigidbody2D.gravityScale = 0;
            Collider2D.isTrigger = true;
            Rigidbody2D.mass = _settings.Mass;
            Rigidbody2D.linearDamping = _settings.LinearDumping;
        }

        /// <summary>
        /// トリガー検出
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.GetComponent<IBallDamageDealer>() is IBallDamageDealer damageDealer) {
                TakeDamage(damageDealer, collision.ClosestPoint(transform.position));
            }
        }

        /// <summary>
        /// トリガー中の処理
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerStay2D(Collider2D collision) {
            if (collision.gameObject.GetComponent<IBallEnter>() is IBallEnter ballEnter) {
                var velocity = ballEnter.OnStayBall(collision.ClosestPoint(transform.position));
                Rigidbody2D.linearVelocity = velocity;
            }
        }

        /// <summary>
        /// トリガーから出たときの処理
        /// </summary>
        /// <param name="collision"></param>
        void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.GetComponent<IBallEnter>() is IBallEnter ballEnter) {
                ballEnter.OnExitBall(collision.ClosestPoint(transform.position));
                Collider2D.isTrigger = false;
                Rigidbody2D.gravityScale = _settings.GravityScale;
            }
        }

        /// <summary>
        /// 衝突検出
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.GetComponent<IBallDamageDealer>() is IBallDamageDealer damageDealer) {
                TakeDamage(damageDealer, collision.GetContact(0).point);
            }
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        private void TakeDamage(IBallDamageDealer damageDealer, Vector2 hitPosition) {
            var (damage, impact) = damageDealer.OnHitBall(hitPosition);
            _health.TakeDamage(damage);

            if (impact > 0) {
                var direction = ((Vector2)transform.position - hitPosition).normalized;
                Rigidbody2D.linearVelocity = Vector2.zero;
                Rigidbody2D.AddForce(direction * impact, ForceMode2D.Impulse);
            }
        }

        void OnDestroy() {
            _health?.Dispose();
        }

    }
}
