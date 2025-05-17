using PinShot.Scenes.MainGame.Player;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    public class MainGameInjector : MonoBehaviour {
        [SerializeField] private PlayerInjector _playerInjector;
        [SerializeField] private PlayerControlSettings _playerControlSettings;

        private void Awake() {
            _playerInjector.Initialize(_playerControlSettings);
        }
    }
}
