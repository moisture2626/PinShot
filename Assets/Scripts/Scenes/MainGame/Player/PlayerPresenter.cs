using PinShot.Database;
using PinShot.Event;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PinShot.Scenes.MainGame.Player {
    /// <summary>
    /// プレイヤーの操作を行うクラス
    /// </summary>
    public class PlayerPresenter : MonoBehaviour {
        [SerializeField] private Transform _leftLimit;
        [SerializeField] private Transform _rightLimit;
        private PlayerControlSettings _playerControlSettings;
        private PlayerView _playerView;
        private MissileLauncher _missileLauncher;
        private PlayerMovement _playerMovement;
        private PlayerShooting _playerShooting;
        [SerializeField] private InputAction _moveAction;
        [SerializeField] private InputAction _shotAction;
        private bool _initialized = false;
        private bool _inputEnabled = false;
        private int _power = 0;
        public void Initialize(PlayerControlSettings playerControlSettings, PlayerView playerView, MissileLauncher missileLauncher) {
            _playerControlSettings = playerControlSettings;
            _playerView = playerView;

            _missileLauncher = missileLauncher;

            _moveAction.Enable();
            _shotAction.Enable();

            _shotAction.performed += OnShotActionPerformed;
            _playerMovement = new PlayerMovement();
            _playerMovement.Initialize(_playerControlSettings, _playerView, _leftLimit, _rightLimit);
            _playerShooting = new PlayerShooting();
            _playerShooting.Initialize(_playerControlSettings, _missileLauncher);

            // ゲームの状態を監視
            EventManager<GameStateEvent>.Subscribe(
                this,
                ev => {
                    _inputEnabled = ev.State == GameState.Play;

                    if (ev.State == GameState.Standby) {
                        _playerShooting.ResetFireInterval();
                        _power = 0;
                    }
                });

            // アイテムの効果を受け取る
            _playerView.ItemEffectObservable
                .Subscribe(effect => {
                    switch (effect.type) {
                        case Item.ItemType.SpeedUp:
                            _playerShooting.AddFireInterval(effect.value);
                            break;
                        case Item.ItemType.PowerUp:
                            _power += (int)effect.value;
                            break;
                        default:
                            break;
                    }
                })
                .AddTo(this);
            _initialized = true;
        }



        private void Update() {

            if (_initialized && _inputEnabled) {
                // マウス位置を常に監視
                Vector2 currentMousePosition = _moveAction.ReadValue<Vector2>();
                _playerMovement.Move(currentMousePosition);
            }
        }


        /// <summary>
        /// 射撃
        /// </summary>
        /// <param name="context"></param>
        private void OnShotActionPerformed(InputAction.CallbackContext context) {
            if (!_initialized || !_inputEnabled) return;
            _playerShooting.Fire(_power);
        }

        private void OnDestroy() {
            _shotAction?.Dispose();
            _moveAction?.Dispose();
        }
    }
}
