using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    public class PlayerInjector : MonoBehaviour {
        [SerializeField] private PlayerPresenter _presenter;
        [SerializeField] private PlayerView _view;
        [SerializeField] private MissileLauncher _missileLauncher;
        public void Initialize() {
            var playerControlSettings = MasterDataManager.GetTable<PlayerControlSettings>();
            var missileSettings = MasterDataManager.GetTable<MissileSettings>();
            _missileLauncher.Initialize(missileSettings);
            _presenter.Initialize(playerControlSettings, _view, _missileLauncher);
        }
    }
}
