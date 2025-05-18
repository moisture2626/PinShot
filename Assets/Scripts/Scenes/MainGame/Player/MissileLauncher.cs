using System.Threading;
using Cysharp.Threading.Tasks;
using PinShot.Database;
using PinShot.Singletons;
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
        public void Fire(int addPower) {
            FireAsync(addPower, destroyCancellationToken).Forget();
        }

        /// <summary>
        /// 発射から爆発まで
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask FireAsync(int addPower, CancellationToken token) {
            var missile = _missilePool.Get();
            missile.SetAddPower(addPower);
            SoundManager.Instance.PlaySE("Shot");
            await missile.FireFlow(Vector2.up, token);

            token.ThrowIfCancellationRequested();
            // ミサイルの発射後、プールに戻す
            _missilePool.Release(missile);

        }

        #region Pool
        private Missile CreateMissile() {
            var missile = Instantiate(_missilePrefab, _missileSpawnPoint.position, Quaternion.identity);
            missile.Initialize(_missileSettings);
            return missile;
        }

        private void OnGetMissile(Missile missile) {
            missile.gameObject.SetActive(true);
            missile.transform.position = _missileSpawnPoint.position;
            missile.Reset();
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
