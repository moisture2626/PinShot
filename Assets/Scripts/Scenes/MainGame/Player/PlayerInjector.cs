using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    public class PlayerInjector : MonoBehaviour {
        [SerializeField] private PlayerPresenter _presenter;
        [SerializeField] private PlayerView _view;
        public void Initialize() {
            var playerControlSettings = MasterDataManager.GetTable<PlayerControlSettings>();
            _presenter.Initialize(playerControlSettings, _view);
        }
    }
}
