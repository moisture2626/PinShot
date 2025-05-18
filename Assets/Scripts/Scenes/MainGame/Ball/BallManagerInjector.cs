using PinShot.Database;
using PinShot.UI;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Ball {
    /// <summary>
    /// BallManagerのDI
    /// </summary>
    public class BallManagerInjector : MonoBehaviour {
        [SerializeField] private BallManager _ballManager;


        public void Initialize(GameUI gameUI) {
            var ballSettings = MasterDataManager.GetTable<BallSettings>();
            var launcherSettings = MasterDataManager.GetTable<BallManagerSettings>();
            _ballManager.Initialize(launcherSettings, ballSettings, gameUI);
        }
    }
}
