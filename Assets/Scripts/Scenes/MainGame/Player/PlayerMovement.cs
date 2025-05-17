using PinShot.Database;
using UnityEngine;

namespace PinShot.Scenes.MainGame.Player {
    /// <summary>
    /// 移動の制御
    /// </summary> 
    public class PlayerMovement {
        private PlayerView _view;
        private PlayerControlSettings _settings;
        private float _leftLimit;
        private float _rightLimit;

        public void Initialize(PlayerControlSettings settings, PlayerView view, Transform leftLimit, Transform rightLimit) {
            _settings = settings;
            _view = view;
            _leftLimit = leftLimit.position.x;
            _rightLimit = rightLimit.position.x;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="mousePosition"></param>
        public void Move(Vector2 mousePosition) {
            // マウス座標をワールド座標に変換
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));

            // X軸のみ移動
            Vector3 targetPosition = new Vector3(worldPosition.x, _view.transform.position.y, _view.transform.position.z);

            // 移動制限の適用
            targetPosition.x = Mathf.Clamp(targetPosition.x, _leftLimit, _rightLimit);

            // 自機を移動
            _view.transform.position = Vector3.Lerp(
                _view.transform.position,
                targetPosition,
                _settings.MoveSpeed * Time.deltaTime
            );

        }
    }
}
