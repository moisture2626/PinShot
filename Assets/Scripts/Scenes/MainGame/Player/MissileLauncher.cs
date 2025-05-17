using System.Threading;
using Cysharp.Threading.Tasks;
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
        public void Initialize() {
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

        private async UniTask FireAsync(CancellationToken token) {
            var missile = _missilePool.Get();
            await missile.FireFlow(Vector2.up, 10, token);
            // ミサイルの発射後、プールに戻す
            _missilePool.Release(missile);
        }

        private Missile CreateMissile() {
            return Instantiate(_missilePrefab, _missileSpawnPoint.position, Quaternion.identity);
        }

        private void OnGetMissile(Missile missile) {
            missile.gameObject.SetActive(true);
            missile.transform.position = _missileSpawnPoint.position;
            missile.Initialize();
        }
        private void OnReleaseMissile(Missile missile) {
            missile.gameObject.SetActive(false);
        }
        private void OnDestroyMissile(Missile missile) {
            Destroy(missile.gameObject);
        }
    }
}
