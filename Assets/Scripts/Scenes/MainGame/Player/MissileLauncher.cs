using UnityEngine;
using UnityEngine.Pool;

namespace PinShot.Scenes.MainGame.Player {
    /// <summary>
    /// ミサイルの管理用
    /// </summary>
    public class MissileLauncher : MonoBehaviour {
        [SerializeField] private GameObject _missilePrefab;
        [SerializeField] private Transform _missileSpawnPoint;

        ObjectPool<MissileView> _missilePool;
        public void Fire() {
            Debug.Log("Missile Fired!");
        }
    }
}
