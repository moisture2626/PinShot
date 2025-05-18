using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PinShot.Singletons {
    /// <summary>
    /// 画面のフェードを制御するクラス
    /// </summary>

    public class ScreenFade : BaseSingleton<ScreenFade> {
        [SerializeField] private Image _fadeImage;

        private CancellationTokenSource _fadeCancellation;

        /// <summary>
        /// フェードアウト
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="color"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask FadeOutAsync(float duration, Color color, CancellationToken token) {
            // すでにフェード中ならキャンセル
            Cancel();
            _fadeCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

            _fadeImage.gameObject.SetActive(true);
            var startColor = color;
            _fadeImage.color = startColor;
            try {
                await _fadeImage.DOColor(color, duration).SetEase(Ease.Linear).WithCancellation(token);
            }
            catch {
                // catchしたら画面を表示
                _fadeImage.color = startColor;
                _fadeImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="color"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask FadeInAsync(float duration, Color color, CancellationToken token) {
            // すでにフェード中ならキャンセル
            Cancel();
            _fadeCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

            _fadeImage.gameObject.SetActive(true);
            var endColor = color;
            endColor.a = 0;
            try {
                await _fadeImage.DOColor(endColor, duration).SetEase(Ease.Linear).WithCancellation(token);
            }
            finally {
                // 最終的に非表示
                _fadeImage.color = Color.clear;
                _fadeImage.gameObject.SetActive(false);
            }
        }

        private void Cancel() {
            _fadeCancellation?.Cancel();
            _fadeCancellation?.Dispose();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            Cancel();
        }
    }
}
