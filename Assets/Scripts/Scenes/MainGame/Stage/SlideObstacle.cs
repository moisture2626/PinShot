using DG.Tweening;
using PinShot.Event;
using UnityEngine;
using VContainer;

namespace PinShot {
    public class SlideObstacle : MonoBehaviour {
        [SerializeField] private float _speed = 1f;
        [SerializeField] private Transform _leftPoint;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private Transform _obstacle;

        private Tween _tween;

        [Inject]
        void Construct() {
            EventManager<GameStateEvent>.Subscribe(ev => {
                if (ev.State == GameState.Play) {
                    ResetPosition();
                    StartMoving();
                }
                else if (ev.State == GameState.Result) {
                    ResetPosition();
                    StopMoving();
                }
            });
        }

        private void ResetPosition() {
            _obstacle.position = _rightPoint.position;
        }

        private void StartMoving() {
            // left と right の間をSinカーブで往復する
            _tween = _obstacle.DOMoveX(_leftPoint.position.x, _speed)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void StopMoving() {
            if (_tween != null && _tween.IsActive()) {
                _tween.Kill();
                _tween = null;
            }
        }
    }
}
