using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    /// <summary>
    /// BallLauncherのDI
    /// </summary>
    public class BallLauncherInjector : MonoBehaviour {
        [SerializeField] private BallLauncher _ballLauncher;

        public void Initialize() {
            var ballSettings = MasterDataManager.GetTable<BallSettings>();
            var launcherSettings = MasterDataManager.GetTable<BallLauncherSettings>();
            _ballLauncher.Initialize(launcherSettings, ballSettings);
        }
    }
}
