using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    public class PlayerInjector : MonoBehaviour {
        [SerializeField] private PlayerPresenter _presenter;
        [SerializeField] private PlayerView _view;
        public void Initialize(PlayerControlSettings playerControlSettings) {
            _presenter.Initialize(playerControlSettings, _view);
        }
    }
}
