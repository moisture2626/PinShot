using PinShot.Database;
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
        [SerializeField] private InputAction _moveAction;
        [SerializeField] private InputAction _shotAction;
        private float _lastShotTime;

        public void Initialize(PlayerControlSettings playerControlSettings, PlayerView playerView) {
            _playerControlSettings = playerControlSettings;
            _playerView = playerView;

            _moveAction.Enable();
            _shotAction.Enable();

            _shotAction.performed += OnShotActionPerformed;
        }



        private void Update() {
            // マウス位置を常に監視
            if (_moveAction != null) {
                Vector2 currentMousePosition = _moveAction.ReadValue<Vector2>();
                Move(currentMousePosition);
            }
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="mousePosition"></param>
        private void Move(Vector2 mousePosition) {
            // マウス座標をワールド座標に変換
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));

            // X軸のみ移動
            Vector3 targetPosition = new Vector3(worldPosition.x, _playerView.transform.position.y, _playerView.transform.position.z);

            // 移動制限の適用
            targetPosition.x = Mathf.Clamp(targetPosition.x, _leftLimit.position.x, _rightLimit.position.x);

            // 自機を移動
            _playerView.transform.position = Vector3.Lerp(
                _playerView.transform.position,
                targetPosition,
                _playerControlSettings.MoveSpeed * Time.deltaTime
            );

            Debug.Log($"Mouse Position: {mousePosition}, Target: {targetPosition}");
        }


        /// <summary>
        /// 射撃
        /// </summary>
        /// <param name="context"></param>
        private void OnShotActionPerformed(InputAction.CallbackContext context) {

        }

        private void OnDestroy() {
            _shotAction?.Dispose();
            _moveAction?.Dispose();
        }
    }
}
