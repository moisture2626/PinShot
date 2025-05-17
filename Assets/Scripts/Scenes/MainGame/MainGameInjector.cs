using PinShot.Scenes.MainGame.Player;
using UnityEngine;

namespace PinShot.Scenes.MainGame {
    /// <summary>
    /// メインゲームシーンのDI
    /// </summary>
    public class MainGameInjector : MonoBehaviour {
        [SerializeField] private PlayerInjector _playerInjector;

        private void Awake() {
            _playerInjector.Initialize();
        }
    }
}
