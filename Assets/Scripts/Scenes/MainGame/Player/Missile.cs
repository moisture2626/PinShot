using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    public class Missile : MonoBehaviour {
        [SerializeField] private SpriteRenderer _missileView;
        public SpriteRenderer MissileView => _missileView;
        [SerializeField] private SpriteRenderer _explosionView;
        public SpriteRenderer ExplosionView => _explosionView;
        private Collider2D _collider2D;
        public Collider2D Collider2D {
            get {
                if (_collider2D == null) {
                    _collider2D = GetComponent<Collider2D>();
                }
                return _collider2D;
            }
        }
        private Rigidbody2D _rigidbody2D;
        public Rigidbody2D Rigidbody2D {
            get {
                if (_rigidbody2D == null) {
                    _rigidbody2D = GetComponent<Rigidbody2D>();
                }
                return _rigidbody2D;
            }
        }

        private MissileSettings _settings;

        // 衝突待機用のUniTaskCompletionSource
        private UniTaskCompletionSource<Collider2D> _triggerTaskSource;

        public void Initialize(MissileSettings settings) {
            _missileView.enabled = true;
            _explosionView.enabled = false;
            _settings = settings;

            _triggerTaskSource = new UniTaskCompletionSource<Collider2D>();
        }

        /// <summary>
        /// 発射から爆発までのフロー
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask FireFlow(Vector2 direction, CancellationToken token) {
            Rigidbody2D.linearVelocity = direction.normalized * _settings.Speed;
            Collider2D.enabled = true;

            // 衝突するまで待機
            await WaitForTrigger(token);
            Rigidbody2D.linearVelocity = Vector2.zero;

            // 爆発
            // 爆発後も当たり判定を残す、エフェクトはさらに長い時間フェードアウトさせる
            await UniTask.WhenAll(
                ExplosionFlow(_settings.ExplosionLifeTime, token),
                ExplosionFadeOutFlow(_settings.ExplosionEffectLifetime, token)
            );
        }

        /// <summary>
        /// 爆発中の当たり判定制御
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask ExplosionFlow(float lifeTime, CancellationToken token) {
            // 衝突すると爆発エフェクトを表示
            _missileView.enabled = false;
            _explosionView.enabled = true;
            await UniTask.Delay((int)(lifeTime * 1000), cancellationToken: token);
            Collider2D.enabled = false;
        }

        /// <summary>
        /// 爆発エフェクトのフェードアウト
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask ExplosionFadeOutFlow(float lifeTime, CancellationToken token) {
            // 1秒にしておく
            _explosionView.color = Color.white;
            await _explosionView.DOFade(0, lifeTime).SetEase(Ease.Linear).WithCancellation(token);
        }

        /// <summary>
        /// 非同期でトリガーを待つ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask<Collider2D> WaitForTrigger(CancellationToken token) {
            _triggerTaskSource = new UniTaskCompletionSource<Collider2D>();

            try {
                return await _triggerTaskSource.Task.AttachExternalCancellation(token);
            }
            catch (System.OperationCanceledException) {
                Debug.Log("トリガー待機がキャンセルされました");
                throw;
            }
        }

        /// <summary>
        /// トリガー検出
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other) {
            // すでに完了していない場合のみCompletionSourceを完了させる
            if (_triggerTaskSource != null && !_triggerTaskSource.Task.Status.IsCompleted()) {
                Debug.Log($"トリガー検出: {other.gameObject.name}");
                _triggerTaskSource.TrySetResult(other);
            }
        }

        void OnDestroy() {

        }
    }
}
