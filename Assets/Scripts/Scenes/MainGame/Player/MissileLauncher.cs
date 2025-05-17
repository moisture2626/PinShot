using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using UnityEngine;
using UnityEngine.Pool;

namespace PinShot.Scenes.MainGame.Player {
    /// <summary>
    /// ミサイルの管理用
    /// </summary>
    public class MissileLauncher : MonoBehaviour {
        [SerializeField] private Missile _missilePrefab;
        [SerializeField] private Transform _missileSpawnPoint;

        ObjectPool<Missile> _missilePool;
        private MissileSettings _missileSettings;
        public void Initialize(MissileSettings missileSettings) {
            _missileSettings = missileSettings;

            _missilePool = new ObjectPool<Missile>(
                CreateMissile,
                OnGetMissile,
                OnReleaseMissile,
                OnDestroyMissile
            );
        }
        public void Fire() {
            FireAsync(destroyCancellationToken).Forget();
        }

        /// <summary>
        /// 発射から爆発まで
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask FireAsync(CancellationToken token) {
            var missile = _missilePool.Get();

            await missile.FireFlow(Vector2.up, token);
            token.ThrowIfCancellationRequested();
            // ミサイルの発射後、プールに戻す
            _missilePool.Release(missile);

        }

        #region Pool
        private Missile CreateMissile() {
            return Instantiate(_missilePrefab, _missileSpawnPoint.position, Quaternion.identity);
        }

        private void OnGetMissile(Missile missile) {
            missile.gameObject.SetActive(true);
            missile.transform.position = _missileSpawnPoint.position;
            missile.Initialize(_missileSettings);
        }
        private void OnReleaseMissile(Missile missile) {
            missile.gameObject.SetActive(false);
        }
        private void OnDestroyMissile(Missile missile) {
            Destroy(missile.gameObject);
        }
        #endregion
    }
}
